using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerUserProfileSynchronizerTests
    {
        private readonly SqlServerFixture fixture;

        public SqlServerUserProfileSynchronizerTests(SqlServerFixture fixture)
        {
            fixture.ClearData();
            this.fixture = fixture;
        }

        [Fact]
        public async Task UpdateUserProfile_WhenNotExists_CreatesRow()
        {
            var userId = "123";

            var userProfile = new UserProfile
            {
                UserId = userId,
                Name = "name",
                Email = "email",
            };

            var synchronizer = new SqlServerUserProfileSynchronizer(fixture.CreateDbContext());
            await synchronizer.UpdateUserProfile(userProfile, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.UserProfiles
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(userProfile.Name);
            data.Email.Should().Be(userProfile.Email);
        }

        [Fact]
        public async Task UpdateExtendedUserProfile_WhenExists_UpdatesRow()
        {
            var userId = "123";

            var userProfile = new UserProfileData
            {
                UserId = userId,
                Name = "original name",
                Email = "original email",
            };

            await fixture.Seed(new[] { userProfile });

            var updatedUserProfile = new UserProfile
            {
                UserId = userId,
                Name = "updated name",
                Email = "updated email",
            };

            var synchronizer = new SqlServerUserProfileSynchronizer(fixture.CreateDbContext());
            await synchronizer.UpdateUserProfile(updatedUserProfile, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.UserProfiles
                .FirstAsync(p => p.UserId == userId);

            data.Name.Should().Be(updatedUserProfile.Name);
            data.Email.Should().Be(updatedUserProfile.Email);
        }
    }
}
