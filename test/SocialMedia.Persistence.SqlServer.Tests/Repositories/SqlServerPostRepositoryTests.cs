using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerPostRepositoryTests
    {
        private readonly SqlServerFixture fixture;

        public SqlServerPostRepositoryTests(SqlServerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task CreatePost_WhenNotExists_CreatesRows()
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = "userId",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());
            await repository.CreatePost(post, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Posts
                .Include(p => p.Content)
                .FirstAsync(p => p.Id == post.Id);

            data.UserId.Should().Be(post.UserId);
            data.Created.Should().Be(post.Created);
            data.Content.Text.Should().Be(post.Text);
        }

        [Fact]
        public async Task CreatePost_WhenExists_Throws()
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = "userId",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var existingPost = new PostData
            {
                Id = post.Id,
                UserId = post.UserId,
                Created = post.Created,
                Content = new PostContentData
                {
                    PostId = post.Id,
                    Text = post.Text,
                }
            };
            await fixture.Seed(new[] { existingPost });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var action = () => repository.CreatePost(post, CancellationToken.None);

            await action.Should().ThrowAsync<ArgumentException>();
        }
    }
}
