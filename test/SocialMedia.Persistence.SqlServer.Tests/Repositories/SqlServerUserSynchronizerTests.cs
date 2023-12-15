using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerUserSynchronizerTests
    {
        private readonly SqlServerFixture fixture;
        private readonly SqlServerUserSynchronizer synchronizer;
        private readonly UserBuilder userBuilder = new();

        public SqlServerUserSynchronizerTests(SqlServerFixture fixture)
        {
            fixture.ClearData();
            this.fixture = fixture;

            synchronizer = new SqlServerUserSynchronizer(fixture.CreateDbContext());
        }

        [Fact]
        public async Task SyncUser_WhenNotExists_CreatesRow()
        {
            var user = userBuilder.CreateUser();

            await synchronizer.SyncUser(user.ToUser(), CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.Id == user.Id!.Value);

            data.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task UpdateExtendedUser_WhenExists_UpdatesRow()
        {
            var user = userBuilder.CreateUser();

            await fixture.Seed(new[] { user.ToUserData() });

            var updatedUser = userBuilder.CreateUser().WithId(user.Id);

            await synchronizer.SyncUser(updatedUser.ToUser(), CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.Id == user.Id!.Value);

            data.Name.Should().Be(updatedUser.Name);
        }
    }
}
