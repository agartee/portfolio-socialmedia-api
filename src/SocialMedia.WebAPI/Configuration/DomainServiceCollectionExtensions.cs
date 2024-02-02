using CommandLine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer;
using SocialMedia.Persistence.SqlServer.Repositories;
using System.Reflection;

namespace SocialMedia.WebAPI.Configuration
{

    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, string[] args)
        {
            var domainAssemblies = new[] { typeof(GetHelpText).Assembly };

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(domainAssemblies);
            });

            services.AddCli(domainAssemblies);
            services.AddDbContexts(configuration, args);
            services.AddRepositories();

            return services;
        }

        private static IServiceCollection AddCli(this IServiceCollection services, Assembly[] domainAssemblies)
        {
            var requestTypes = GetMediatrRequestTypes(domainAssemblies);
            var cliRequestTypes = requestTypes
                .Where(t => Attribute.GetCustomAttribute(t, typeof(VerbAttribute)) != null)
                .ToArray();

            services.AddTransient<ICliRequestBuilder>(services =>
                new CliRequestBuilder(requestTypes));
            services.AddSingleton(new HelpTextConfiguration(cliRequestTypes));

            return services;
        }

        private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration, string[] args)
        {
            var dbConnectionStringName = new Parser(settings => { settings.CaseSensitive = false; })
                .GetConnectionStringName(args);
            services.AddDbContext<SocialMediaDbContext>(options =>
                options.UseSqlServer(configuration[$"connectionStrings:{dbConnectionStringName}"]));

            return services;
        }

        private static string GetConnectionStringName(this Parser parser, string[] args)
        {
            var dbConnectionString = "database";

            parser.ParseArguments<CommandLineArgs>(args)
                .WithParsed(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.DbConnectionStringName))
                    {
                        dbConnectionString = x.DbConnectionStringName;
                    }
                });

            return dbConnectionString;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IPostRepository, SqlServerPostRepository>();
            services.AddTransient<IUserSynchronizer, SqlServerUserSynchronizer>();

            return services;
        }

        private static Type[] GetMediatrRequestTypes(Assembly[] domainAssemblies)
        {
            return domainAssemblies.SelectMany(assembly => assembly.ExportedTypes
                .Where(t => typeof(IBaseRequest).IsAssignableFrom(t.GetTypeInfo())))
                .ToArray();
        }
    }
}
