using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerUserSynchronizerTests
    {
        private readonly SqlServerFixture fixture;

        public SqlServerUserSynchronizerTests(SqlServerFixture fixture)
        {
            fixture.ClearData();
            this.fixture = fixture;
        }

        [Fact]
        public async Task UpdateUser_WhenNotExists_CreatesRow()
        {
            var userId = "123";

            var user = new User
            {
                UserId = userId,
                Name = "name"
            };

            var synchronizer = new SqlServerUserSynchronizer(fixture.CreateDbContext());
            await synchronizer.SyncUser(user, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task UpdateExtendedUser_WhenExists_UpdatesRow()
        {
            var userId = "123";

            var user = new UserData
            {
                UserId = userId,
                Name = "original name",
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            await fixture.Seed(new[] { user });

            var updatedUser = new User
            {
                UserId = userId,
                Name = "updated name"
            };

            var synchronizer = new SqlServerUserSynchronizer(fixture.CreateDbContext());
            await synchronizer.SyncUser(updatedUser, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(updatedUser.Name);
        }
    }
}
