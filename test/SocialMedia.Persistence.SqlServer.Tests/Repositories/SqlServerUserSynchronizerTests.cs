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
                Name = "name",
                Email = "email",
            };

            var synchronizer = new SqlServerUserSynchronizer(fixture.CreateDbContext());
            await synchronizer.SyncUser(user, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(user.Name);
            data.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task UpdateExtendedUser_WhenExists_UpdatesRow()
        {
            var userId = "123";

            var user = new UserData
            {
                UserId = userId,
                Name = "original name",
                Email = "original email",
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            await fixture.Seed(new[] { user });

            var updatedUser = new User
            {
                UserId = userId,
                Name = "updated name",
                Email = "updated email",
            };

            var synchronizer = new SqlServerUserSynchronizer(fixture.CreateDbContext());
            await synchronizer.SyncUser(updatedUser, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.Users
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(updatedUser.Name);
            data.Email.Should().Be(updatedUser.Email);
        }
    }
}
