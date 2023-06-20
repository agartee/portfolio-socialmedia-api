using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IExtendedUserProfileRepository
    {
        Task<ExtendedUserProfile?> GetExtendedUserProfile(string userId, CancellationToken cancellationToken);
        Task<ExtendedUserProfile> CreateExtendedUserProfile(ExtendedUserProfile userProfile, CancellationToken cancellationToken);
        Task<ExtendedUserProfile> UpdateExtendedUserProfile(ExtendedUserProfile userProfile, CancellationToken cancellationToken);
    }
}
