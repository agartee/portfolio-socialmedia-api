using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IUserRepository
    {
        Task<User> GetUser(string userId, CancellationToken cancellationToken);
        Task<User> UpdateUser(User user, CancellationToken cancellationToken);
    }
}
