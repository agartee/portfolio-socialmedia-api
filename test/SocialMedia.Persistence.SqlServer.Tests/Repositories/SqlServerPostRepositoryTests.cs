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
            var userProfile = new UserProfileData
            {
                UserId = "user1",
                DisplayName = "User 1"
            };

            await fixture.Seed(new[] { userProfile });

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());
            var result = await repository.CreatePost(post, CancellationToken.None);

            result.Id.Should().Be(post.Id);
            result.Author.Should().Be(userProfile.DisplayName);
            result.Created.Should().Be(post.Created);
            result.Text.Should().Be(post.Text);

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
                UserId = "123",
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
                },
                UserProfile = new UserProfileData
                {
                    UserId = post.UserId
                }
            };

            await fixture.Seed(new[] { existingPost });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var action = () => repository.CreatePost(post, CancellationToken.None);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"*{"An item with the same key has already been added"}*")
                .WithMessage($"*{post.Id}*"); ;
        }

        [Fact]
        public async Task GetAllPosts_ReturnsAllPostsWithUserDisplayNameWhenAvailable_OrderedByCreatedDesc()
        {
            var user1Id = "123";
            var user1DisplayName = "User 1";
            var user2Id = "456";
            var post1Id = Guid.NewGuid();
            var post2Id = Guid.NewGuid();

            var post1 = new PostData
            {
                Id = post1Id,
                UserId = user1Id,
                Created = new DateTime(2023, 1, 1),
                Content = new PostContentData
                {
                    PostId = post1Id,
                    Text = "text 1",
                },
                UserProfile = new UserProfileData
                {
                    UserId = user1Id,
                    DisplayName = user1DisplayName
                }
            };

            var post2 = new PostData
            {
                Id = post2Id,
                UserId = user2Id,
                Created = new DateTime(2023, 1, 2),
                Content = new PostContentData
                {
                    PostId = post2Id,
                    Text = "text 2",
                }
            };

            await fixture.Seed(new object[] { post1, post2 });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var results = await repository.GetAllPosts(CancellationToken.None);

            results.Should().HaveCount(2);

            var result1 = results.First();
            result1.Author.Should().Be(user2Id);
            result1.Created.Should().Be(post2.Created);
            result1.Text.Should().Be(post2.Content.Text);

            var result2 = results.Last();
            result2.Author.Should().Be(user1DisplayName);
            result2.Created.Should().Be(post1.Created);
            result2.Text.Should().Be(post1.Content.Text);
        }
    }
}
