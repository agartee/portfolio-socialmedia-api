using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer.Repositories;

namespace SocialMedia.WebAPI.Configuration
{
    public static class SqlServerPersistenceServicesExtensions
    {
        public static IServiceCollection AddSqlServerPersistenceServices(this IServiceCollection services)
        {
            services.AddTransient<IPostRepository, SqlServerPostRepository>();
            services.AddTransient<IExtendedUserProfileRepository, SqlServerUserProfileRepository>();

            return services;
        }
    }
}
