using Microsoft.Net.Http.Headers;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.Auth0;
using SocialMedia.Persistence.Auth0.Configuration;

namespace SocialMedia.WebAPI.Configuration
{
    public static class Auth0ServicesExtensions
    {
        public static IServiceCollection AddAuth0ManagementServices(this IServiceCollection services,
            ConfigurationManager config)
        {
            services.AddSingleton(new Auth0ManagementAPIConfiguration
            {
                Audience = config["userManagement:authentication:audience"],
                ClientId = config["userManagement:authentication:clientId"],
                ClientSecret = config["userManagement:authentication:clientSecret"],
            });

            services.AddTransient<AuthenticatedHttpMessageHandler>();
            services.AddHttpClient<AuthenticatedHttpMessageHandler>(client =>
            {
                var baseUrl = config["authentication:authority"];
                if (baseUrl != null)
                    client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddTransient<IUserProfileRepository, Auth0ManagementAPIClient>();
            services.AddHttpClient<IUserProfileRepository, Auth0ManagementAPIClient>(client =>
            {
                var baseUrl = config["userManagement:authentication:audience"];
                if (baseUrl != null)
                    client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

            return services;
        }
    }
}
