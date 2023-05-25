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

        [Fact]
        public async Task GetAllPosts_ReturnsAllPosts()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var existingPosts = new[]
            {
                new PostData
                {
                    Id = id1,
                    UserId = "user1",
                    Created = DateTime.UtcNow,
                    Content = new PostContentData
                    {
                        PostId = id1,
                        Text = "text 1",
                    }
                },
                new PostData
                {
                    Id = id2,
                    UserId = "user2",
                    Created = DateTime.UtcNow,
                    Content = new PostContentData
                    {
                        PostId = id2,
                        Text = "text 2",
                    }
                }
            };

            await fixture.Seed(existingPosts);

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var results = await repository.GetAllPosts(CancellationToken.None);

            results.Should().HaveCount(2);

            var result1 = results.First(p => p.Id == id1);
            result1.UserId.Should().Be(existingPosts[0].UserId);
            result1.Created.Should().Be(existingPosts[0].Created);
            result1.Text.Should().Be(existingPosts[0].Content.Text);

            var result2 = results.First(p => p.Id == id2);
            result2.UserId.Should().Be(existingPosts[1].UserId);
            result2.Created.Should().Be(existingPosts[1].Created);
            result2.Text.Should().Be(existingPosts[1].Content.Text);
        }
    }
}
