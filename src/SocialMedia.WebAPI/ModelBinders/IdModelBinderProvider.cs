using Microsoft.AspNetCore.Mvc.ModelBinding;
using SocialMedia.Domain.Models;
using System.Reflection;

namespace SocialMedia.WebAPI.ModelBinders
{
    public class IdModelBinderProvider : IModelBinderProvider
    {
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
            return Id.SupportedTypes.Any(
                t => typeof(Id<>).MakeGenericType(t).IsAssignableFrom(objectType.GetTypeInfo())
            );
        }
    }
}
