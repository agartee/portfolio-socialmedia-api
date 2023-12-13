using Microsoft.AspNetCore.Mvc.ModelBinding;
using SocialMedia.Domain.Models;
using System.Reflection;

namespace SocialMedia.WebAPI.Binders
{
    public class IdModelBinderProvider : IModelBinderProvider
    {
        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(Guid),
            typeof(string)
        };

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (CanConvert(context.Metadata.ModelType))
                return new IdModelBinder();

            return null;
        }

        private static bool CanConvert(Type objectType)
        {
            return SupportedTypes.Any(
                t => typeof(Id<>).MakeGenericType(t).IsAssignableFrom(objectType.GetTypeInfo())
            );
        }
    }
}
