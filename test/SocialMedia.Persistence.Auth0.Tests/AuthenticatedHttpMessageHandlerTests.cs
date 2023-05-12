using FluentAssertions;
using Moq;
using Moq.Protected;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using SocialMedia.Persistence.Auth0.Tests.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0.Tests
{
    public class AuthenticatedHttpMessageHandlerTests
    {
        [Fact]
        public async Task SendAsync_WhenNoTokenIsCached_RequestsAuthTokenAndAttachesAuthHeader()
        {
            var authConfig = new Auth0ManagementAPIConfiguration
            {
                Audience = "audience",
                ClientId = "client",
                ClientSecret = "secret",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                new AuthResponse
                {
                    TokenType = "test",
                    AccessToken = "ABC123",
                    Scope = "test",
                    ExpiresIn = 86400
                });

            var wrappedHandler = new HttpStatusOkHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://test.com", CancellationToken.None);

            VerifyAuthRequestMessage(new AuthRequest
            {
                Audience = authConfig.Audience,
                ClientId = authConfig.ClientId,
                ClientSecret = authConfig.ClientSecret,
                GrantType = GrantTypes.CLIENT_CREDENTIALS
            });

            wrappedHandler.LastRequest.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));


            void VerifyAuthRequestMessage(AuthRequest expectedRequest)
            {
                authHttpMessageHandler.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Matches(HttpMethod.Post, "https://test.com/oauth/token", expectedRequest)),
                        ItExpr.IsAny<CancellationToken>());
            }
        }

        [Fact]
        public async Task SendAsync_WhenAuthAPIReturnValueIsNotRecognized_Throws()
        {
            var authConfig = new Auth0ManagementAPIConfiguration
            {
                Audience = "audience",
                ClientId = "client",
                ClientSecret = "secret",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", "null");

            var wrappedHandler = new HttpStatusOkHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            var action = () => httpClient.GetAsync("http://test.com", CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        [Fact]
        public async Task SendAsync_WhenTokenIsCached_AttachesAuthHeaderButDoesNotRequestNewAuthToken()
        {
            var authToken = new AuthToken
            {
                TokenType = "test",
                AccessToken = "ABC123",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/");

            var wrappedHandler = new HttpStatusOkHandler();
            var handler = new AuthenticatedHttpMessageHandler(
                authHttpClient, Auth0ManagementAPIConfiguration.Empty(), defaultToken: authToken)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://test.com", CancellationToken.None);

            wrappedHandler.LastRequest.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));

            authHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        private (HttpClient, Mock<HttpMessageHandler>) CreateMockHttpClient(string baseUrl, object? response = null)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response as string ?? JsonSerializer.Serialize(response)),
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri(baseUrl);

            return (httpClient, httpMessageHandler);
        }

        public class HttpStatusOkHandler : DelegatingHandler
        {
            public HttpRequestMessage LastRequest { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
