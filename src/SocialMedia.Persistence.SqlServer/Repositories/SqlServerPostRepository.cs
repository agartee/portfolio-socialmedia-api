using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Extensions;

namespace SocialMedia.Persistence.SqlServer.Repositories
{
    public class SqlServerPostRepository : IPostRepository
    {
        private readonly SocialMediaDbContext dbContext;

        public SqlServerPostRepository(SocialMediaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PostInfo> CreatePost(Post post, CancellationToken cancellationToken)
        {
            dbContext.Posts.Add(post.ToPostData());

            await dbContext.SaveChangesAsync();

            var postData = await dbContext.Posts
                .Include(p => p.Content)
                .Include(p => p.User)
                .SingleAsync(p => p.Id == post.Id.Value);

            return postData.ToPostInfo();
        }

        public async Task<PostInfo> DemandPost(PostId id, CancellationToken cancellationToken)
        {
            var postsData = await dbContext.Posts
                .Include(post => post.Content)
                .Include(post => post.User)
                .Where(p => p.Id == id.Value)
                .SingleAsync();

            return postsData.ToPostInfo();
        }

        public async Task<IEnumerable<PostInfo>> GetAllPosts(CancellationToken cancellationToken)
        {
            var postsData = await dbContext.Posts
                .Include(post => post.Content)
                .Include(post => post.User)
                .OrderByDescending(post => post.Created)
                .ToListAsync();

            return postsData.Select(p => p.ToPostInfo());
        }
    }
}
