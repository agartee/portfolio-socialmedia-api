using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Repositories
{
    public class SqlServerUserProfileRepository : IExtendedUserProfileRepository
    {
        private readonly SocialMediaDbContext dbContext;

        public SqlServerUserProfileRepository(SocialMediaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ExtendedUserProfile> GetExtendedUserProfile(string userId, CancellationToken cancellationToken)
        {
            var userProfileData = await dbContext.UserProfiles
                .SingleOrDefaultAsync(p => p.UserId == userId);

            if (userProfileData == null)
                throw new EntityNotFoundException(nameof(ExtendedUserProfile), userId);

            return new ExtendedUserProfile
            {
                UserId = userProfileData.UserId,
                DisplayName = userProfileData.DisplayName
            };
        }

        public async Task<ExtendedUserProfile> CreateExtendedUserProfile(ExtendedUserProfile userProfile, CancellationToken cancellationToken)
        {
            dbContext.UserProfiles.Add(new UserProfileData
            {
                UserId = userProfile.UserId,
                DisplayName = userProfile.DisplayName
            });

            await dbContext.SaveChangesAsync();

            return userProfile;
        }

        public async Task<ExtendedUserProfile> UpdateExtendedUserProfile(ExtendedUserProfile userProfile, CancellationToken cancellationToken)
        {
            var userProfileData = await dbContext.UserProfiles
                .SingleOrDefaultAsync(p => p.UserId == userProfile.UserId);

            if (userProfileData == null)
                throw new EntityNotFoundException(nameof(ExtendedUserProfile), userProfile.UserId);

            userProfileData.DisplayName = userProfile.DisplayName;

            await dbContext.SaveChangesAsync();

            return userProfile;
        }
    }
}
