using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.Auth0.Configuration;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SocialMedia.Persistence.Auth0
{
    public class Auth0ManagementAPIClient : IUserProfileRepository
    {
        private readonly HttpClient httpClient;
        private readonly Auth0ManagementAPIConfiguration config;

        public Auth0ManagementAPIClient(HttpClient httpClient, Auth0ManagementAPIConfiguration config)
        {
            this.httpClient = httpClient;
            this.config = config;
        }

        public async Task<UserProfile> GetUserProfile(string userId, CancellationToken cancellationToken)
        {
            string url = $"users/{userId}";

            var httpResponse = await httpClient.GetAsync(url, cancellationToken);
            var userResponse = await TryDeserialize(httpResponse.Content, cancellationToken);

            if (userResponse == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            return new UserProfile
            {
                Id = userResponse.Id,
                Name = userResponse.Name,
                Nickname = userResponse.Nickname,
                Email = userResponse.Email,
            };
        }

        public async Task<UserProfile> UpdateUserProfile(UserProfile userProfile, CancellationToken cancellationToken)
        {
            var payload = new UserRequest
            {
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email
            };

            var url = $"users/{userProfile.Id}";

            var httpResponse = await httpClient.PatchAsJsonAsync(url, payload, cancellationToken);
            var userResponse = await TryDeserialize(httpResponse.Content, cancellationToken);

            if (userResponse == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            return new UserProfile
            {
                Id = userResponse.Id,
                Name = userResponse.Name,
                Nickname = userResponse.Nickname,
                Email = userResponse.Email,
            };
        }

        private static async Task<UserResponse?> TryDeserialize(HttpContent content,
            CancellationToken cancellationToken)
        {
            try
            {
                return await content.ReadFromJsonAsync<UserResponse>(
                    cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
                return null;
            }

        }
    }
}
