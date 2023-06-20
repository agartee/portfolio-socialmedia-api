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
        public async Task GetBasicUserProfile_QueriesAPIAndReturnsResult()
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

            var result = await apiClient.GetUserProfile(id, CancellationToken.None);

            UserProfile expectedResult = new UserProfile
            {
                UserId = apiResponse.user_id,
                Name = apiResponse.name,
                Email = apiResponse.email
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
        public async Task GetBasicUserProfile_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var id = "id";

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/", responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.GetUserProfile(id, CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        [Fact]
        public async Task UpdateBasicUserProfile_PatchesAPIAndReturnsResult()
        {
            var id = "123";

            var userProfile = new UserProfile
            {
                UserId = id,
                Name = "name",
                Email = "original@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", new
                {
                    user_id = id,
                    name = userProfile.Name,
                    email = userProfile.Email
                });

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var result = await apiClient.UpdateUserProfile(userProfile, CancellationToken.None);

            result.Should().Be(userProfile);

            var expectedRequest = new UserRequest
            {
                Name = userProfile.Name,
                Email = userProfile.Email
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
        public async Task UpdateBasicUserProfile_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var userProfile = new UserProfile
            {
                UserId = "id",
                Name = "name",
                Email = "original@here.com"
            };

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.UpdateUserProfile(userProfile, CancellationToken.None);

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
