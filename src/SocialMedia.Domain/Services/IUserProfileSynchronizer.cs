using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IUserProfileSynchronizer
    {
        Task<UserProfile> UpdateUserProfile(UserProfile userProfile, CancellationToken cancellationToken);
    }
}
