using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Concurrent;
using System.Reflection;

namespace SocialMedia.WebAPI.Binders
{
    public class IdModelBinder : IModelBinder
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> ConstructorCache =
            new();

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var fieldName = bindingContext.FieldName;
            var fieldValue = bindingContext.ValueProvider.GetValue(fieldName);

            if (fieldValue == ValueProviderResult.None)
                return Task.CompletedTask;
            else
                bindingContext.ModelState.SetModelValue(fieldName, fieldValue);

            var value = fieldValue.FirstValue;
            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            try
            {
                var typeToConvert = bindingContext.ModelType;
                var constructor = GetConstructor(typeToConvert);

                var currentIdValueType = value.GetType();
                var targetIdValueType = constructor.GetParameters().First().ParameterType;

                object result;
                if (currentIdValueType == targetIdValueType)
                    result = constructor.Invoke(new[] { value });

                if (targetIdValueType == typeof(Guid))
                    result = constructor.Invoke(new object[] { new Guid(value) });
                else
                    result = constructor.Invoke(
                        new object[] { Convert.ChangeType(value, targetIdValueType) }
                    );

                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private static ConstructorInfo GetConstructor(Type typeToConvert)
        {
            return ConstructorCache.GetOrAdd(
                typeToConvert,
                t =>
                {
                    return typeToConvert
                            .GetConstructors()
                            .Where(c => c.GetParameters().Length == 1)
                            .FirstOrDefault()
                        ?? throw new InvalidOperationException(
                            $"Unable to bind model, {typeToConvert}. Cannot find suitable constructor."
                        );
                }
            );
        }
    }
}
