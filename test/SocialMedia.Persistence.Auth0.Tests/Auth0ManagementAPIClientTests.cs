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
        public async Task GetUserProfile_QueriesAPIAndReturnsResult()
        {
            var id = "id";

            var apiResponse = new UserResponse
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "me@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", apiResponse);

            var apiClient = new Auth0ManagementAPIClient(httpClient,
                Auth0ManagementAPIConfiguration.Empty());

            var result = await apiClient.GetUserProfile(id, CancellationToken.None);

            result.Should().Be(new UserProfile
            {
                Id = apiResponse.Id,
                Name = apiResponse.Name,
                Nickname = apiResponse.Nickname,
                Email = apiResponse.Email
            });

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Get, $"https://test.com/users/{id}")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetUserProfile_WhenAPIReturnValueIsNotRecognized_Throws()
        {
            var id = "id";

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", "null");

            var apiClient = new Auth0ManagementAPIClient(httpClient,
                Auth0ManagementAPIConfiguration.Empty());

            var action = () => apiClient.GetUserProfile(id, CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        [Fact]
        public async Task UpdateUserProfile_PatchesAPIAndReturnsResult()
        {
            var userProfile = new UserProfile
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "original@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                new UserResponse
                {
                    Id = userProfile.Id,
                    Name = userProfile.Name,
                    Nickname = userProfile.Nickname,
                    Email = userProfile.Email
                });

            var apiClient = new Auth0ManagementAPIClient(httpClient,
                Auth0ManagementAPIConfiguration.Empty());

            var result = await apiClient.UpdateUserProfile(userProfile, CancellationToken.None);

            result.Should().Be(userProfile);

            var expectedRequest = new UserRequest
            {
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email
            };

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Patch, $"https://test.com/users/{userProfile.Id}", expectedRequest)),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateUserProfile_WhenAPIReturnValueIsNotRecognized_Throws()
        {
            var userProfile = new UserProfile
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "original@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                "null");

            var apiClient = new Auth0ManagementAPIClient(httpClient,
                Auth0ManagementAPIConfiguration.Empty());

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
