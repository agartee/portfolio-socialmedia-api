using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0.Tests.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class Auth0ManagementAPIClientTestExtensions
    {
        internal static bool Matches<T>(this HttpRequestMessage message, HttpMethod httpMethod, string url, T content) where T : class
        {
            return message.Matches(httpMethod, url)
                && (content == null || ValidateRequestBody<T>(message.Content!, content));
        }

        internal static bool Matches(this HttpRequestMessage message, HttpMethod httpMethod, string url)
        {
            return message.Method == httpMethod
                && message.RequestUri!.Equals(url);
        }

        private static bool ValidateRequestBody<T>(HttpContent content, T expectedRequest) where T : class
        {
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualRequest = JsonSerializer.Deserialize<T>(jsonContent);

            return actualRequest!.Equals(expectedRequest);
        }
    }
}
