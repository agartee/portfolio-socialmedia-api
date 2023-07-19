using FluentAssertions;
using Moq;
using Moq.Protected;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using SocialMedia.Persistence.Auth0.Tests.Extensions;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0.Tests
{
    public class Auth0ManagementAPIClientTests
    {
        [Fact]
        public async Task GetUser_QueriesAPIAndReturnsUser()
        {
            var id = "123";

            var apiResponse = new
            {
                user_id = id,
                name = "name",
                nickname = "nickname",
                email = "me@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", apiResponse);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var result = await apiClient.GetUser(id, CancellationToken.None);

            User expectedResult = new User
            {
                UserId = apiResponse.user_id,
                Name = apiResponse.name
            };

            result.Should().Be(expectedResult);

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Get, $"https://test.com/users/{id}")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetUser_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var id = "id";

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/", responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.GetUser(id, CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        [Fact]
        public async Task UpdateUser_PatchesAPIAndReturnsResult()
        {
            var id = "123";

            var user = new User
            {
                UserId = id,
                Name = "name"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", new
                {
                    user_id = id,
                    name = user.Name
                });

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var result = await apiClient.UpdateUser(user, CancellationToken.None);

            result.Should().Be(user);

            var expectedRequest = new UserRequest
            {
                Name = user.Name
            };

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Patch, $"https://test.com/users/{id}", expectedRequest)),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateUser_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.UpdateUser(user, CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        private (HttpClient, Mock<HttpMessageHandler>) CreateMockHttpClient(string baseUrl, object response)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(response as string ?? JsonSerializer.Serialize(response)),
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri(baseUrl);

            return (httpClient, httpMessageHandler);
        }
    }
}
