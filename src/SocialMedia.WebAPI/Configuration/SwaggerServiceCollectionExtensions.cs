using Microsoft.OpenApi.Models;
using SocialMedia.WebAPI.Swagger;

namespace SocialMedia.WebAPI.Configuration
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorization token; will be passed as \"Bearer {token}\".",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement { { CreateRequirementKey(), Array.Empty<string>() } });

                options.SchemaFilter<CorrectSchemaFilter>();
            });

            return services;
        }

        private static OpenApiSecurityScheme CreateRequirementKey()
        {
            return new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
        }
    }
}
