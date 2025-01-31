using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.SqlServer;

namespace SocialMedia.WebAPI.Configuration
{
    public static class DbMigrationExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();

            dbContext.Database.Migrate();

            return host;
        }
    }
}
