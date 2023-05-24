using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SocialMedia.Persistence.SqlServer.Tests.Fixtures
{
    [ExcludeFromCodeCoverage]
    public class SqlServerFixture
    {
        public SocialMediaDbContext CreateDbContext([CallerMemberName] string? caller = null)
        {
            return new SocialMediaDbContext(new DbContextOptionsBuilder<SocialMediaDbContext>()
                .UseInMemoryDatabase(databaseName: caller ?? "SocialMediaTests")
                .Options);
        }

        public async Task Seed(object[] entities, [CallerMemberName] string? caller = null)
        {
            var dbContext = CreateDbContext(caller);

            foreach (var entity in entities)
                dbContext.Add(entity);

            await dbContext.SaveChangesAsync();
        }
    }

    [CollectionDefinition("SqlServerTestCollection")]
    public class SqlServerTestCollection : ICollectionFixture<SqlServerFixture>
    {
    }
}
