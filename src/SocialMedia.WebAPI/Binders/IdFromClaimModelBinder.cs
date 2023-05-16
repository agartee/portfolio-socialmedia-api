using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text.Json;

namespace SocialMedia.WebAPI.Binders
{
    public class IdFromClaimModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var id = GetIdFromClaim(bindingContext.HttpContext);
            var body = await GetRequestBody(bindingContext.HttpContext.Request);
            var dict = string.IsNullOrEmpty(body)
                ? new Dictionary<string, object>()
                : DeserializeToDictionary(body);

            dict["id"] = id;

            var model = JsonSerializer.Deserialize(
                JsonSerializer.Serialize(dict),
                bindingContext.ModelType,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            bindingContext.Result = ModelBindingResult.Success(model);
        }

        private static string GetIdFromClaim(HttpContext httpContext)
        {
            var idClaim = httpContext.User.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier);

            return idClaim.Value;
        }

        private async static Task<string> GetRequestBody(HttpRequest request)
        {
            using var streamReader = new StreamReader(request.Body);
            return await streamReader.ReadToEndAsync();
        }

        private static Dictionary<string, object> DeserializeToDictionary(string body)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (dict == null)
                    return new Dictionary<string, object>();

                return dict;
            }
            catch (Exception ex) // due to System.Text.Json throwing internal JsonReaderException
            {
                throw new InvalidOperationException("Cannot deserialize model from request body.", ex);
            }
        }
    }
}
