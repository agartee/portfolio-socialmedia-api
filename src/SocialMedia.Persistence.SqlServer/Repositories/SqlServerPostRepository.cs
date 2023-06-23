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

        public async Task<PostInfo> CreatePost(Post post, CancellationToken cancellationToken)
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

            var resultData = await dbContext.Posts
                .Include(p => p.Content)
                .Include(p => p.User)
                .SingleAsync(p => p.Id == post.Id);

            return new PostInfo
            {
                Id = resultData.Id,
                Author = resultData.User!.Name,
                Created = resultData.Created,
                Text = resultData.Content.Text
            };
        }

        public async Task<IEnumerable<PostInfo>> GetAllPosts(CancellationToken cancellationToken)
        {
            var postsData = await dbContext.Posts
                .Include(post => post.Content)
                .Include(post => post.User)
                .OrderByDescending(post => post.Created)
                .ToListAsync();

            return postsData.Select(p => new PostInfo
            {
                Id = p.Id,
                Author = p.User!.Name,
                Created = p.Created,
                Text = p.Content.Text
            });
        }
    }
}
