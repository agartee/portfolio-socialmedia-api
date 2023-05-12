using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetUserProfile(string userId, CancellationToken cancellationToken);
        Task<UserProfile> UpdateUserProfile(UserProfile userProfile, CancellationToken cancellationToken);
    }
}
