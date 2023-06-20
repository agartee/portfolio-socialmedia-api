using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Repositories
{
    public class SqlServerUserProfileSynchronizer : IUserProfileSynchronizer
    {
        private readonly SocialMediaDbContext dbContext;

        public SqlServerUserProfileSynchronizer(SocialMediaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserProfile> UpdateUserProfile(UserProfile userProfile, CancellationToken cancellationToken)
        {
            var userProfileData = await dbContext.UserProfiles
                .SingleOrDefaultAsync(p => p.UserId == userProfile.UserId);

            if (userProfileData == null)
                CreateUserProfile(userProfile);
            else
                UpdateUserProfile(userProfile, userProfileData);

            await dbContext.SaveChangesAsync(cancellationToken);

            return userProfile;
        }

        private void CreateUserProfile(UserProfile userProfile)
        {
            var now = DateTime.UtcNow;

            dbContext.UserProfiles.Add(new UserProfileData
            {
                UserId = userProfile.UserId,
                Name = userProfile.Name,
                Email = userProfile.Email,
                Created = now,
                LastUpdated = now
            });
        }

        private static void UpdateUserProfile(UserProfile userProfile, UserProfileData userProfileData)
        {
            userProfileData.Name = userProfile.Name;
            userProfileData.Email = userProfile.Email;
            userProfileData.LastUpdated = DateTime.UtcNow;
        }
    }
}
