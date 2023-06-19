using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.Persistence.SqlServer.Repositories;
using SocialMedia.Persistence.SqlServer.Tests.Fixtures;

namespace SocialMedia.Persistence.SqlServer.Tests.Repositories
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerUserProfileRepositoryTests
    {
        private readonly SqlServerFixture fixture;

        public SqlServerUserProfileRepositoryTests(SqlServerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetExtendedUserProfile_WhenExists_ReturnsUserProfile()
        {
            var userId = "123";

            var existingUserProfile = new UserProfileData
            {
                UserId = userId,
                DisplayName = "display name"
            };

            await fixture.Seed(new[] { existingUserProfile });

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());
            var result = await repository.GetExtendedUserProfile(userId, CancellationToken.None);

            result!.UserId.Should().Be(existingUserProfile.UserId);
            result!.DisplayName.Should().Be(existingUserProfile.DisplayName);
        }

        [Fact]
        public async Task GetExtendedUserProfile_WhenNotExists_ReturnsNull()
        {
            var userId = "123";

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());
            var result = await repository.GetExtendedUserProfile(userId, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateExtendedUserProfile_WhenNotExists_CreatesRow()
        {
            var userId = "123";

            var userProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "display name"
            };

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());
            await repository.CreateExtendedUserProfile(userProfile, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.UserProfiles
                .FirstAsync(p => p.UserId == userId);

            data.UserId.Should().Be(userProfile.UserId);
            data.DisplayName.Should().Be(userProfile.DisplayName);
        }

        [Fact]
        public async Task CreateExtendedUserProfile_WhenExists_Throws()
        {
            var userId = "123";

            var existingUserProfile = new UserProfileData
            {
                UserId = userId,
                DisplayName = "existing profile display name"
            };

            await fixture.Seed(new[] { existingUserProfile });

            var newUserProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "new profile display name"
            };

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());

            var action = () => repository.CreateExtendedUserProfile(newUserProfile, CancellationToken.None);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"*{"An item with the same key has already been added"}*")
                .WithMessage($"*{userId}*");
        }

        [Fact]
        public async Task UpdateExtendedUserProfile_WhenExists_UpdatesRow()
        {
            var userId = "123";

            var existingUserProfile = new UserProfileData
            {
                UserId = userId,
                DisplayName = "original display name"
            };

            await fixture.Seed(new[] { existingUserProfile });

            var updatedUserProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "updated display name"
            };

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());
            await repository.UpdateExtendedUserProfile(updatedUserProfile, CancellationToken.None);

            using var dbContext = fixture.CreateDbContext();
            var data = await dbContext.UserProfiles
                .FirstAsync(p => p.UserId == userId);

            data.UserId.Should().Be(updatedUserProfile.UserId);
            data.DisplayName.Should().Be(updatedUserProfile.DisplayName);
        }

        [Fact]
        public async Task UpdateExtendedUserProfile_WhenNotExists_Throws()
        {
            var userId = "123";

            var userProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "display name"
            };

            var repository = new SqlServerUserProfileRepository(fixture.CreateDbContext());
            var action = () => repository.UpdateExtendedUserProfile(userProfile, CancellationToken.None);

            await action.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage($"*{userId}*")
                .WithMessage($"*{nameof(ExtendedUserProfile)}*");
        }
    }
}
