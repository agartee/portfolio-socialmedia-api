using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Services
{
    public interface IPostRepository
    {
        Task<PostInfo> CreatePost(Post post, CancellationToken cancellationToken);
        Task<PostInfo> DemandPost(PostId id, CancellationToken cancellationToken);
        Task<IEnumerable<PostInfo>> GetAllPosts(CancellationToken cancellationToken);
    }
}
