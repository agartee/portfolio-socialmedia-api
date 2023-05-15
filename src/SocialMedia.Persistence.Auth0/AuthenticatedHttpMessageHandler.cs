using SocialMedia.Persistence.Auth0.Configuration;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0
{
    public class AuthenticatedHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpClient httpClient;
        private readonly Auth0ManagementAPIConfiguration config;

        public AuthenticatedHttpMessageHandler(HttpClient httpClient,
            Auth0ManagementAPIConfiguration config, AuthToken? initToken = null)
        {
            this.httpClient = httpClient;
            this.config = config;
            CachedToken = initToken;
        }

        public AuthToken? CachedToken { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = CachedToken ?? await RequestNewToken(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                token.TokenType, token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<AuthToken> RequestNewToken(CancellationToken cancellationToken)
        {
            var url = "oauth/token";

            var httpResponse = await httpClient.PostAsJsonAsync(url,
                new AuthRequest
                {
                    Audience = config.Audience,
                    ClientId = config.ClientId,
                    ClientSecret = config.ClientSecret,
                    GrantType = GrantTypes.CLIENT_CREDENTIALS
                },
                cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                throw new AuthenticationFailedException();

            var token = await DeserializeResponseBody(httpResponse.Content, cancellationToken);

            if (token == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            CachedToken = token;
            return token;
        }

        private async Task<AuthToken?> DeserializeResponseBody(HttpContent content, CancellationToken cancellationToken)
        {
            try
            {
                var authResponse = await content.ReadFromJsonAsync<AuthResponse>(
                    cancellationToken: cancellationToken);

                return authResponse != null
                    ? new AuthToken
                    {
                        TokenType = authResponse.TokenType,
                        AccessToken = authResponse.AccessToken
                    } : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
