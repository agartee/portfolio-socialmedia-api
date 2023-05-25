using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Repositories
{
    public class SqlServerPostRepository : IPostRepository
    {
        private readonly SocialMediaDbContext dbContext;

        public SqlServerPostRepository(SocialMediaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePost(Post post, CancellationToken cancellationToken)
        {
            var postData = new PostData
            {
                Id = post.Id,
                UserId = post.UserId,
                Created = post.Created,
                Content = new PostContentData
                {
                    PostId = post.Id,
                    Text = post.Text
                }
            };

            dbContext.Posts.Add(postData);

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Post>> GetAllPosts(CancellationToken cancellationToken)
        {
            var postsData = await dbContext.Posts
                .Include(post => post.Content)
                .OrderByDescending(post => post.Created)
                .ToListAsync();

            return postsData.Select(p => new Post
            {
                Id = p.Id,
                UserId = p.UserId,
                Created = p.Created,
                Text = p.Content.Text
            });
        }
    }
}
