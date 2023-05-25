using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IPostRepository
    {
        Task CreatePost(Post post, CancellationToken cancellationToken);
        Task<IEnumerable<Post>> GetAllPosts(CancellationToken cancellationToken);
    }
}
