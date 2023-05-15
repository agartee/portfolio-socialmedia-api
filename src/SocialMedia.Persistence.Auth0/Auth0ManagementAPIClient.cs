using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Extensions;
using SocialMedia.Persistence.Auth0.Models;
using System.Net.Http.Json;

namespace SocialMedia.Persistence.Auth0
{
    public class Auth0ManagementAPIClient : IUserProfileRepository
    {
        private readonly HttpClient httpClient;

        public Auth0ManagementAPIClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<UserProfile> GetUserProfile(string userId, CancellationToken cancellationToken)
        {
            string url = $"users/{userId}";

            var httpResponse = await httpClient.GetAsync(url, cancellationToken);
            var userResponse = await httpResponse.Content.TryReadFromJsonAsync<UserResponse>(cancellationToken);

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
            var userResponse = await httpResponse.Content.TryReadFromJsonAsync<UserResponse>(cancellationToken);

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
    }
}
