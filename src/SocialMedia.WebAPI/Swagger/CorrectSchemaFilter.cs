using Microsoft.OpenApi.Models;
using SocialMedia.Domain.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;

namespace SocialMedia.WebAPI.Swagger
{
    public class CorrectSchemaFilter : ISchemaFilter
    {
        private static readonly Dictionary<Type, string> supportedTypes = new Dictionary<Type, string>
        {
            [typeof(short)] = "number",
            [typeof(int)] = "number",
            [typeof(long)] = "number",
            [typeof(ulong)] = "number",
            [typeof(decimal)] = "number",
            [typeof(float)] = "number",
            [typeof(double)] = "number",
            [typeof(char)] = "string",
            [typeof(Guid)] = "string",
            [typeof(string)] = "string"
        };

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (IsIdType(context.Type))
            {
                schema.Type = GetBackingTypeDescription(context.Type);
                schema.Properties.Clear();
            }
        }

        private bool IsIdType(Type contextType)
        {
            return supportedTypes.Keys.Any(t => typeof(Id<>).MakeGenericType(t).IsAssignableFrom(contextType));
        }

        private string? GetBackingTypeDescription(Type contextType)
        {
            var baseType = GetGenericIdBackingType(contextType);
            var backingType = baseType.GetGenericArguments().Single();
            supportedTypes.TryGetValue(backingType, out var openApiSchemaTypeName);

            return openApiSchemaTypeName;
        }

        private static Type GetGenericIdBackingType(Type type)
        {
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType && type.GetGenericTypeDefinition() == typeof(Id<>))
                {
                    return currentType.GetGenericArguments()[0];
                }
                currentType = currentType.BaseType;
            }

            throw new UnreachableException();
        }
    }
}
