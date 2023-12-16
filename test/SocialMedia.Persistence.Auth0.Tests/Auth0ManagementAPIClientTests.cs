using FluentAssertions;
using Moq;
using Moq.Protected;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using SocialMedia.Persistence.Auth0.Tests.Extensions;
using SocialMedia.TestUtilities.Builders;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0.Tests
{
    public class Auth0ManagementAPIClientTests
    {
        private readonly UserBuilder userBuilder = new();

        [Fact]
        public async Task GetUser_QueriesAPIAndReturnsUser()
        {
            var idValue = "123";

            var apiResponse = new
            {
                user_id = idValue,
                name = "name",
                nickname = "nickname",
                email = "me@here.com"
            };

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", apiResponse);

            var apiClient = new Auth0ManagementAPIClient(httpClient);
            var result = await apiClient.GetUser(new UserId(idValue), CancellationToken.None);

            var expectedResult = userBuilder.CreateUser()
                .WithId(new UserId(apiResponse.user_id))
                .WithName(apiResponse.name)
                .ToUser();

            result.Should().Be(expectedResult);

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Get, $"https://test.com/users/{idValue}")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetUser_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var userId = new UserId("id");

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/", responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.GetUser(userId, CancellationToken.None);

            await action.Should().ThrowAsync<CannotDeserializeResponseException>();
        }

        [Fact]
        public async Task UpdateUser_PatchesAPIAndReturnsResult()
        {
            var user = userBuilder.CreateUser();

            var (httpClient, httpMessageHandler) = CreateMockHttpClient(
                baseUrl: "https://test.com/", new
                {
                    user_id = user.Id!.Value,
                    name = user.Name
                });

            var apiClient = new Auth0ManagementAPIClient(httpClient);
            var result = await apiClient.UpdateUser(user.ToUser(), CancellationToken.None);

            result.Should().Be(user.ToUser());

            var expectedRequest = new UserRequest
            {
                Name = user.Name
            };

            httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Matches(HttpMethod.Patch, $"https://test.com/users/{user.Id}", expectedRequest)),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateUser_WhenAuthResponseBodyIsInvalid_Throws(string responseBody)
        {
            var user = userBuilder.CreateUser();

            var (httpClient, _) = CreateMockHttpClient(
                baseUrl: "https://test.com/",
                responseBody);

            var apiClient = new Auth0ManagementAPIClient(httpClient);

            var action = () => apiClient.UpdateUser(user.ToUser(), CancellationToken.None);

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
