using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialMedia.Persistence.SqlServer.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SocialMedia.Persistence.SqlServer.Tests.Fixtures
{
    [ExcludeFromCodeCoverage]
    public class SqlServerFixture
    {
        private readonly IConfiguration config;

        public SqlServerFixture()
        {
            config = new ConfigurationBuilder()
                .AddUserSecrets<SqlServerFixture>()
                .AddEnvironmentVariables()
                .Build();

            //var configurationDbContext = CreateDbContext();
            //configurationDbContext.Database.EnsureDeleted();
            //configurationDbContext.Database.EnsureCreated();
        }

        public SocialMediaDbContext CreateDbContext()
        {
            return new SocialMediaDbContext(new DbContextOptionsBuilder<SocialMediaDbContext>()
                .UseSqlServer(config.GetConnectionString("testDatabase")).Options);
        }

        public async Task Seed(object[] entities, [CallerMemberName] string? caller = null)
        {
            var dbContext = CreateDbContext();

            foreach (var entity in entities)
                dbContext.Add(entity);

            await dbContext.SaveChangesAsync();
        }

        public void ClearData()
        {
            var dbContext = CreateDbContext();

            var sqlCommandList = CreateList(
                new { Order = 1, SqlCommand = $"DELETE FROM [{SocialMediaDbContext.SCHEMA_NAME}].[{PostContentData.TABLE_NAME}]" },
                new { Order = 2, SqlCommand = $"DELETE FROM [{SocialMediaDbContext.SCHEMA_NAME}].[{PostData.TABLE_NAME}]" },
                new { Order = 3, SqlCommand = $"DELETE FROM [{SocialMediaDbContext.SCHEMA_NAME}].[{UserData.TABLE_NAME}]" });

            var sql = string.Join(Environment.NewLine, sqlCommandList
                .OrderBy(item => item.Order)
                .Select(item => item.SqlCommand)
                .ToArray());

            dbContext.Database.ExecuteSqlRaw(sql);
        }

        private static List<T> CreateList<T>(params T[] elements)
        {
            return new List<T>(elements);
        }
    }

    [CollectionDefinition("SqlServerTestCollection")]
    public class SqlServerTestCollection : ICollectionFixture<SqlServerFixture>
    {
    }
}
