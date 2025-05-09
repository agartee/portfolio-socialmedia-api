using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;
using SocialMedia.TestUtilities;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection"), UseMappingContextScope]
    public class SqlServerPostRepositoryTests
    {
        private readonly SqlServerFixture fixture;
        private readonly SqlServerPostRepository repository;
        private readonly PostBuilder postBuilder = new();

        public SqlServerPostRepositoryTests(SqlServerFixture fixture)
        {
            fixture.ClearData();
            this.fixture = fixture;

            repository = new SqlServerPostRepository(fixture.CreateDbContext());
        }

        [Fact]
        public async Task CreatePost_WhenNotExists_CreatesRowsAndReturnsPost()
        {
            var post = postBuilder.CreatePost();

            await fixture.Seed(new[] { post.Author!.ToUserData() });

            var result = await repository.CreatePost(post.ToPost(), CancellationToken.None);

            result.Should().Be(post.ToPostInfo());

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Posts
                .Include(p => p.Content)
                .FirstAsync(p => p.Id == post.Id!.Value);

            data.AuthorUserId.Should().Be(post.Author!.Id!.Value);
            data.Created.Should().Be(post.Created);
            data.Content.Text.Should().Be(post.Text);
        }

        [Fact]
        public async Task DemandPost_WhenExists_ReturnsPost()
        {
            var post = postBuilder.CreatePost();

            await fixture.Seed(new[] { post.ToPostData() });

            var result = await repository.DemandPost(post.Id!, CancellationToken.None);

            result.Should().Be(post.ToPostInfo());
        }

        [Fact]
        public async Task DemandPost_WhenNotExists_Throws()
        {
            var postId = PostId.NewId();

            var action = () => repository.DemandPost(postId, CancellationToken.None);

            await action.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage($"*{nameof(PostInfo)}*")
                .WithMessage($"*{postId}*");
        }

        [Fact]
        public async Task GetAllPosts_ReturnsAllPosts_OrderedByCreatedDesc()
        {
            var post1 = postBuilder.CreatePost();
            var post2 = postBuilder.CreatePost()
                .WithAuthor(post1.Author);

            var post1Data = post1.ToPostData();
            var post2Data = post2.ToPostData();

            await fixture.Seed(post1Data, post2Data);

            var results = await repository.GetAllPosts(CancellationToken.None);

            results.Should().HaveCount(2);

            var result1 = results.First(p => p.Id == post1.Id);
            result1.Author.Should().Be(post1.Author!.Name);
            result1.Created.Should().Be(post1.Created);
            result1.Text.Should().Be(post1.Text);

            var result2 = results.First(p => p.Id == post2.Id);
            result2.Author.Should().Be(post1.Author!.Name);
            result2.Created.Should().Be(post2.Created);
            result2.Text.Should().Be(post2.Text);
        }
    }
}
