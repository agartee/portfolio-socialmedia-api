using Microsoft.OpenApi.Models;
using SocialMedia.WebAPI.Swagger;

namespace SocialMedia.WebAPI.Configuration
{
    public static class SwaggerServiceCollectionExtensions
    {
        private const string SecurityDefinitionName = "Bearer";
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(SecurityDefinitionName, new OpenApiSecurityScheme
                {
                    Description = @"Authorization token; will be passed as ""Bearer {token}"".
                                    When pasting a value in this field, be sure to
                                    <strong>exclude</strong> the ""Bearer \"" prefix.",
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
                    Id = SecurityDefinitionName
                }
            };
        }
    }
}
