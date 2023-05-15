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
        public async Task SendAsync_RequestsAuthTokenAndAttachesAuthHeaderToWrappedHttpRequest()
        {
            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://auth.com/", CreateTestResponseMessage(
                    HttpStatusCode.OK,
                    new AuthToken
                    {
                        TokenType = "test",
                        AccessToken = "ABC123"
                    }));

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };

            var httpClient = new HttpClient(handler);

            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            wrappedHandler.LastRequest!.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "ABC123"));

            var expectedAuthTokenRequest = new AuthRequest
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
                    m.Matches(HttpMethod.Post, "https://auth.com/oauth/token", expectedAuthTokenRequest)),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WhenServiceInitializedWithToken_AttachesCachedAuthTokenAuthHeaderToWrappedHttpRequest()
        {
            var initialAuthToken = new AuthToken
            {
                TokenType = "test",
                AccessToken = "ABC123",
            };

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://auth.com/");

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig,
                initToken: initialAuthToken)
            {
                InnerHandler = wrappedHandler
            };

            var httpClient = new HttpClient(handler);

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
        public async Task SendAsync_WhenUnauthorizedResponseReceivedFromWrappedHttpHandler_UpdatesCachedAuthTokenAndRetries()
        {
            var initialAuthToken = new AuthToken
            {
                TokenType = "test",
                AccessToken = "INIT_TOKEN",
            };

            var (authHttpClient, _) = CreateMockHttpMessageHandler(
                baseUrl: "https://auth.com/", CreateTestResponseMessage(
                    HttpStatusCode.OK,
                    new AuthToken
                    {
                        TokenType = "test",
                        AccessToken = "REFRESHED_TOKEN",
                    }));

            var firstResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            var secondResponse = new HttpResponseMessage(HttpStatusCode.OK);

            var wrappedHandler = new SomeOtherHttpRequestHandler(firstResponse, secondResponse);
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig,
                initToken: initialAuthToken)
            {
                InnerHandler = wrappedHandler
            };

            var httpClient = new HttpClient(handler);

            await httpClient.GetAsync("http://anything.com", CancellationToken.None);

            wrappedHandler.LastRequest!.Headers.Authorization.Should().Be(
                new AuthenticationHeaderValue("test", "REFRESHED_TOKEN"));
        }

        [Fact]
        public async Task SendAsync_WhenAuthResponseIsNotSuccess_Throws()
        {

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://auth.com/", CreateTestResponseMessage(
                HttpStatusCode.BadRequest));

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
        public async Task SendAsync_WhenAuthResponseBodyIsNotAuthToken_Throws(string authResponseBody)
        {
            var authResponseMessage = CreateTestResponseMessage(
                HttpStatusCode.OK, authResponseBody);

            var (authHttpClient, authHttpMessageHandler) = CreateMockHttpMessageHandler(
                baseUrl: "https://auth.com/", authResponseMessage);

            var wrappedHandler = new SomeOtherHttpRequestHandler();
            var handler = new AuthenticatedHttpMessageHandler(authHttpClient, authConfig)
            {
                InnerHandler = wrappedHandler
            };

            var httpClient = new HttpClient(handler);

            var action = () => httpClient.GetAsync("http://anything.com", CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        private (HttpClient, Mock<HttpMessageHandler>) CreateMockHttpMessageHandler(string baseUrl,
            params HttpResponseMessage[] responseMessages)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();

            var mockSetup = httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            foreach (var responseMessage in responseMessages)
                mockSetup.ReturnsAsync(responseMessage);

            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri(baseUrl);

            return (httpClient, httpMessageHandler);
        }

        private static HttpResponseMessage CreateTestResponseMessage(HttpStatusCode statusCode, object? response = null)
        {
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(response as string ?? JsonSerializer.Serialize(response)),
            };
        }

        public class SomeOtherHttpRequestHandler : DelegatingHandler
        {
            private readonly Queue<HttpResponseMessage> responses;

            public SomeOtherHttpRequestHandler(params HttpResponseMessage[] responses)
            {
                this.responses = new Queue<HttpResponseMessage>(responses);
            }

            public HttpRequestMessage? LastRequest { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                LastRequest = request;

                if (responses.Count > 0)
                    return Task.FromResult(responses.Dequeue());

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
