using FluentAssertions;
using Moq;
using Moq.Protected;
using SocialMedia.Persistence.Auth0.Configuration;
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
        private readonly Auth0ManagementAPIConfiguration authConfig = new Auth0ManagementAPIConfiguration
        {
            Audience = "audience",
            ClientId = "client",
            ClientSecret = "secret",
        };

        [Fact]
        public async Task SendAsync_WhenTokenIsNotCached_RequestsAuthTokenAndAttachesAuthHeader()
        {
            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/",
                new AuthResponse
                {
                    TokenType = "test",
                    AccessToken = "ABC123",
                    Scope = "test",
                    ExpiresIn = 86400
                });

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            wrappedHandler.LastRequest!.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));

            var expectedAuthRequest = new AuthRequest
            {
                Audience = authConfig.Audience,
                ClientId = authConfig.ClientId,
                ClientSecret = authConfig.ClientSecret,
                GrantType = GrantTypes.CLIENT_CREDENTIALS
            };

            authHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Post, "https://test.com/oauth/token", expectedAuthRequest)),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WhenTokenIsNotCached_CachesNextToken()
        {
            var (authHttpClient, _) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/",
                new AuthResponse
                {
                    TokenType = "test",
                    AccessToken = "ABC123",
                    Scope = "test",
                    ExpiresIn = 86400
                });

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            handler.CachedToken.Should().NotBeNull();
        }

        [Fact]
        public async Task SendAsync_WhenTokenIsCached_AttachesAuthHeaderButDoesNotRequestNewAuthToken()
        {
            var existingAuthToken = new AuthToken
            {
                TokenType = "test",
                AccessToken = "ABC123",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/");

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig,
                initToken: existingAuthToken)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            wrappedHandler.LastRequest!.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));

            authHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WhenTokenIsCachedAndNotAuthorizedResponseReceivedFromWrappedAPICall_UpdatesCachedTokenAndRetries()
        {
            var existingAuthToken = new AuthToken
            {
                TokenType = "test",
                AccessToken = "ABC123",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/");

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig,
                initToken: existingAuthToken)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            // don't care about response for this test
            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            wrappedHandler.LastRequest!.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));

            authHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WhenAuthResponseIsNotSuccess_Throws()
        {
            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/",
                statusCode: HttpStatusCode.BadRequest);

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            var action = () => httpClient.GetAsync("http://anything.com", CancellationToken.None);

            await action.Should().ThrowAsync<AuthenticationFailedException>();
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendAsync_WhenAuthResponseBodyIsInvalid_Throws(string authResponseBody)
        {
            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://test.com/", authResponseBody);

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };
            var httpClient = new HttpClient(handler);

            var action = () => httpClient.GetAsync("http://anything.com", CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        private HttpClient CreateHttpClient(string baseUrl, Mock<HttpMessageHandler> httpMessageHandler)
        {
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri(baseUrl);

            return httpClient;
        }

        private (HttpClient, Mock<HttpMessageHandler>) CreateMockHttpMessageHandler(string baseUrl,
            object? response = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(response as string ?? JsonSerializer.Serialize(response)),
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri(baseUrl);

            return (httpClient, httpMessageHandler);
        }

        public class SomeOtherHttpRequestHandler : DelegatingHandler
        {
            public HttpRequestMessage? LastRequest { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
