using SocialMedia.Domain.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SocialMedia.WebAPI.JsonConverters
{
    public class IdJsonConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return Id.SupportedTypes.Any(t => typeof(Id<>).MakeGenericType(t).IsAssignableFrom(typeToConvert));
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var constructor = GetConstructor(typeToConvert);
            var valueType = constructor.GetParameters().First().ParameterType;
            var value = JsonSerializer.Deserialize(ref reader, valueType, options);

            return constructor.Invoke(new[] { value });
        }

        private static ConstructorInfo GetConstructor(Type typeToConvert)
        {
            return typeToConvert.GetConstructors().Where(c => c.GetParameters().Length == 1).FirstOrDefault()
                ?? throw new InvalidOperationException($"Unable to deserialize {typeToConvert}. Cannot find suitable constructor.");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var valueType = value.GetType().GetProperty("Value")!.GetValue(value);
            JsonSerializer.Serialize(writer, valueType, options);
        }
    }
}
