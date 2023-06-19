using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.Auth0.Exceptions;
using SocialMedia.Persistence.Auth0.Extensions;
using SocialMedia.Persistence.Auth0.Models;
using System.Net.Http.Json;

namespace SocialMedia.Persistence.Auth0
{
    public class Auth0ManagementAPIClient : IBasicUserProfileRepository
    {
        private readonly HttpClient httpClient;

        public Auth0ManagementAPIClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<BasicUserProfile> GetBasicUserProfile(string userId, CancellationToken cancellationToken)
        {
            string url = $"users/{userId}";

            var httpResponse = await httpClient.GetAsync(url, cancellationToken);
            var userResponse = await httpResponse.Content.TryReadFromJsonAsync<UserResponse>(cancellationToken);

            if (userResponse == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            return new BasicUserProfile
            {
                UserId = userResponse.Id,
                Name = userResponse.Name,
                Nickname = userResponse.Nickname,
                Email = userResponse.Email,
            };
        }

        public async Task<BasicUserProfile> UpdateBasicUserProfile(BasicUserProfile userProfile, CancellationToken cancellationToken)
        {
            var payload = new UserRequest
            {
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email
            };

            var url = $"users/{userProfile.UserId}";

            var httpResponse = await httpClient.PatchAsJsonAsync(url, payload, cancellationToken);
            var userResponse = await httpResponse.Content.TryReadFromJsonAsync<UserResponse>(cancellationToken);

            if (userResponse == null)
                throw new CannotDeserializeResponseException(url, typeof(UserResponse));

            return new BasicUserProfile
            {
                UserId = userResponse.Id,
                Name = userResponse.Name,
                Nickname = userResponse.Nickname,
                Email = userResponse.Email,
            };
        }
    }
}
