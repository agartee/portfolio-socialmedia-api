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
                Author = "userId",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());
            await repository.CreatePost(post, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Posts
                .Include(p => p.Content)
                .FirstAsync(p => p.Id == post.Id);

            data.UserId.Should().Be(post.Author);
            data.Created.Should().Be(post.Created);
            data.Content.Text.Should().Be(post.Text);
        }

        [Fact]
        public async Task CreatePost_WhenExists_Throws()
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Author = "userId",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var existingPost = new PostData
            {
                Id = post.Id,
                UserId = post.Author,
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
        public async Task GetAllPosts_ReturnsAllPostsWithUserDisplayNameWhenPresent_OrderedByCreatedDesc()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var post1 = new PostData
            {
                Id = id1,
                UserId = "user1",
                Created = new DateTime(2023, 1, 1),
                Content = new PostContentData
                {
                    PostId = id1,
                    Text = "text 1",
                },
                UserProfile = new UserProfileData
                {
                    UserId = "user1",
                    DisplayName = "User 1"
                }
            };

            var post2 = new PostData
            {
                Id = id2,
                UserId = "user2",
                Created = new DateTime(2023, 1, 2),
                Content = new PostContentData
                {
                    PostId = id2,
                    Text = "text 2",
                }
            };

            await fixture.Seed(new object[] { post1, post2 });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var results = await repository.GetAllPosts(CancellationToken.None);

            results.Should().HaveCount(2);

            var result1 = results.First();
            result1.Author.Should().Be(post2.UserId);
            result1.Created.Should().Be(post2.Created);
            result1.Text.Should().Be(post2.Content.Text);

            var result2 = results.Last();
            result2.Author.Should().Be(post1.UserProfile.DisplayName);
            result2.Created.Should().Be(post1.Created);
            result2.Text.Should().Be(post1.Content.Text);
        }
    }
}
