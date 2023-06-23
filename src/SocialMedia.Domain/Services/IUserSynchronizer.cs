using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IUserSynchronizer
    {
        Task<User> SyncUser(User user, CancellationToken cancellationToken);
    }
}
