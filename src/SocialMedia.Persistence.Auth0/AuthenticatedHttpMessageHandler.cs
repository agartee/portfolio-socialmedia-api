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

        private Task<AuthToken> tokenTask;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public AuthenticatedHttpMessageHandler(HttpClient httpClient,
            Auth0ManagementAPIConfiguration config, AuthToken? initToken = null)
        {
            this.httpClient = httpClient;
            this.config = config;

            tokenTask = initToken != null
                ? Task.FromResult(initToken)
                : RefreshToken();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await SendWithToken(request, cancellationToken, GetToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                response = await SendWithToken(request, cancellationToken, RefreshToken);

            return response;
        }

        private async Task<HttpResponseMessage> SendWithToken(HttpRequestMessage request,
            CancellationToken cancellationToken, Func<Task<AuthToken>> getToken)
        {
            var token = await getToken();

            request.Headers.Authorization = new AuthenticationHeaderValue(
                token.TokenType, token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<AuthToken> GetToken()
        {
            return await tokenTask;
        }

        private async Task<AuthToken> RefreshToken()
        {
            await semaphore.WaitAsync();

            try
            {
                tokenTask = RequestToken();
                return await tokenTask;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<AuthToken> RequestToken()
        {
            var url = "oauth/token";

            var httpResponse = await httpClient.PostAsJsonAsync(url,
                new AuthRequest
                {
                    Audience = config.Audience,
                    ClientId = config.ClientId,
                    ClientSecret = config.ClientSecret,
                    GrantType = GrantTypes.CLIENT_CREDENTIALS
                });

            if (!httpResponse.IsSuccessStatusCode)
                throw new AuthenticationFailedException();

            var token = await TryDeserialize(httpResponse.Content);

            if (token == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            return token;
        }

        private async Task<AuthToken?> TryDeserialize(HttpContent content)
        {
            try
            {
                var authResponse = await content.ReadFromJsonAsync<AuthResponse>();

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
