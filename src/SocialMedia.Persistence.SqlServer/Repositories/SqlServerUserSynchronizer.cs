using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Repositories
{
    public class SqlServerUserSynchronizer : IUserSynchronizer
    {
        private readonly SocialMediaDbContext dbContext;

        public SqlServerUserSynchronizer(SocialMediaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> SyncUser(User user, CancellationToken cancellationToken)
        {
            var userData = await dbContext.Users
                .SingleOrDefaultAsync(p => p.UserId == user.UserId);

            if (userData == null)
                CreateUser(user);
            else
                UpdateUser(user, userData);

            await dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        private void CreateUser(User user)
        {
            var now = DateTime.UtcNow;

            dbContext.Users.Add(new UserData
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Created = now,
                LastUpdated = now
            });
        }

        private static void UpdateUser(User user, UserData userData)
        {
            userData.Name = user.Name;
            userData.Email = user.Email;
            userData.LastUpdated = DateTime.UtcNow;
        }
    }
}
