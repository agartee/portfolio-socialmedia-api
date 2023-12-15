namespace SocialMedia.WebAPI.Configuration
{
    public static class CorsServiceCollectionExtensions
    {
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                var origins = configuration.GetSection("cors:allowedOrigins").Get<string[]>();

                if (origins != null)
                {
                    options.AddPolicy(name: CorsPolicies.AllowedOrigins,
                    policy =>
                    {
                        policy.WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .AllowAnyMethod();
                    });
                }
            });

            return services;
        }
    }

    public static class CorsPolicies
    {
        public const string AllowedOrigins = "allowedOrigins";
    }
}
