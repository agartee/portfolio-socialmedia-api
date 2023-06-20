using FluentAssertions;
using Microsoft.Data.SqlClient;
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
            fixture.ClearData();
            this.fixture = fixture;
        }

        [Fact]
        public async Task CreatePost_WhenNotExists_CreatesRowsAndReturnsPost()
        {
            var userId = "123";
            var userProfile = new UserProfileData
            {
                UserId = userId,
                Name = "User 1",
                Email = "email",
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            await fixture.Seed(new[] { userProfile });

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());
            var result = await repository.CreatePost(post, CancellationToken.None);

            result.Id.Should().Be(post.Id);
            result.Author.Should().Be(userProfile.Name);
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
            var id = Guid.NewGuid();

            var post = new Post
            {
                Id = id,
                UserId = "123",
                Text = "text",
                Created = DateTime.UtcNow,
            };

            var existingPost = new PostData
            {
                Id = id,
                UserId = post.UserId,
                Created = post.Created,
                Content = new PostContentData
                {
                    PostId = post.Id,
                    Text = post.Text,
                },
                UserProfile = new UserProfileData
                {
                    UserId = post.UserId,
                    Name = "User 1",
                    Email = "email",
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                }
            };

            await fixture.Seed(new[] { existingPost });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var action = () => repository.CreatePost(post, CancellationToken.None);

            (await action.Should().ThrowAsync<DbUpdateException>())
                .WithInnerException<SqlException>()
                .WithMessage($"*{"Cannot insert duplicate key"}*")
                .WithMessage($"*{post.Id}*"); ;
        }

        [Fact]
        public async Task GetAllPosts_ReturnsAllPosts_OrderedByCreatedDesc()
        {
            var userId = "123";
            var post1Id = Guid.NewGuid();
            var post2Id = Guid.NewGuid();

            var userProfile = new UserProfileData
            {
                UserId = userId,
                Name = "User 1",
                Email = "email",
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            var post1 = new PostData
            {
                Id = post1Id,
                UserId = userId,
                Created = new DateTime(2023, 1, 1),
                Content = new PostContentData
                {
                    PostId = post1Id,
                    Text = "text 1",
                }
            };

            var post2 = new PostData
            {
                Id = post2Id,
                UserId = userId,
                Created = new DateTime(2023, 1, 2),
                Content = new PostContentData
                {
                    PostId = post2Id,
                    Text = "text 2",
                }
            };

            await fixture.Seed(new object[] { userProfile, post1, post2 });

            var repository = new SqlServerPostRepository(fixture.CreateDbContext());

            var results = await repository.GetAllPosts(CancellationToken.None);

            results.Should().HaveCount(2);

            var result1 = results.First();
            result1.Author.Should().Be(userProfile.Name);
            result1.Created.Should().Be(post2.Created);
            result1.Text.Should().Be(post2.Content.Text);

            var result2 = results.Last();
            result2.Author.Should().Be(userProfile.Name);
            result2.Created.Should().Be(post1.Created);
            result2.Text.Should().Be(post1.Content.Text);
        }
    }
}
