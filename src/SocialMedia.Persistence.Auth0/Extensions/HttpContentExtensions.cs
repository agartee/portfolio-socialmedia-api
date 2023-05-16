using System.Net.Http.Json;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T?> TryReadFromJsonAsync<T>(this HttpContent content,
            CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                return await content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
