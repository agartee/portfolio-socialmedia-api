using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using System.Net.Http.Json;

namespace SocialMedia.Persistence.Auth0
{
    public class AuthenticatedHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpClient httpClient;
        private readonly Auth0ManagementAPIConfiguration config;
        private AuthToken? token;

        public AuthenticatedHttpMessageHandler(HttpClient httpClient,
            Auth0ManagementAPIConfiguration config, AuthToken? defaultToken = null)
        {
            this.httpClient = httpClient;
            this.config = config;
            token = defaultToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = (await GetToken(cancellationToken))
                .ToHeaderValue();

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<AuthToken> GetToken(CancellationToken cancellationToken)
        {
            return token ?? await RequestNewToken(cancellationToken);
        }

        private async Task<AuthToken> RequestNewToken(CancellationToken cancellationToken)
        {
            var body = new AuthRequest
            {
                Audience = config.Audience,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret,
                GrantType = GrantTypes.CLIENT_CREDENTIALS
            };

            var url = "oauth/token";
            var httpResponse = await httpClient.PostAsJsonAsync(url, body,
                cancellationToken);
            var response = await httpResponse.Content.ReadFromJsonAsync<AuthResponse>(
                cancellationToken: cancellationToken);

            if (response == null)
                throw new CannotDeserializeResponseException(url, nameof(UserResponse));

            token = new AuthToken
            {
                TokenType = response.TokenType,
                AccessToken = response.AccessToken
            };

            return token;
        }
    }
}
