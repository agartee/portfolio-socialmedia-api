using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IBasicUserProfileRepository
    {
        Task<BasicUserProfile> GetBasicUserProfile(string userId, CancellationToken cancellationToken);
        Task<BasicUserProfile> UpdateBasicUserProfile(BasicUserProfile userProfile, CancellationToken cancellationToken);
    }
}
