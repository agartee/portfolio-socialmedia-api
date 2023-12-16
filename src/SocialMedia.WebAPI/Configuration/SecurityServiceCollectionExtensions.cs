using Microsoft.AspNetCore.Authentication.JwtBearer;
using SocialMedia.Domain.Services;
using SocialMedia.WebAPI.Security;

namespace SocialMedia.WebAPI.Configuration
{
    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddSecirity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = configuration["authentication:authority"];
                options.Audience = configuration["authentication:audience"];
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                {
                    policy.RequireClaim("permissions", "admin");
                });
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserContext, HttpUserContext>();

            return services;
        }
    }
}
