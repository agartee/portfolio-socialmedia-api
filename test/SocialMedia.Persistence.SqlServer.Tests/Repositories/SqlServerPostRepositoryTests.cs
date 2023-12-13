using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;
using SocialMedia.Persistence.SqlServer.Tests.TestUtilities;
using SocialMedia.TestUtilities.Builders;
using static SocialMedia.Persistence.SqlServer.Tests.TestUtilities.PostExtensions;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerPostRepositoryTests
    {
        private readonly SqlServerFixture fixture;
        private readonly SqlServerPostRepository repository;
        private readonly PostBuilder postBuilder = new();
        private readonly UserBuilder userBuilder = new();

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
        public async Task GetAllPosts_ReturnsAllPosts_OrderedByCreatedDesc()
        {
            var post1 = postBuilder.CreatePost();
            var post2 = postBuilder.CreatePost()
                .WithAuthor(post1.Author);

            await fixture.Seed(new object[]
            {
                post1.Author!.ToUserData(),
                post1.ToPostData(),
                post2.ToPostData()
            });

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
