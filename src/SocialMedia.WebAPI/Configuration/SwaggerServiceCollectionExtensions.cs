using Microsoft.OpenApi.Models;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Swagger;
using System.Reflection;
using System.Xml.Linq;

namespace SocialMedia.WebAPI.Configuration
{
    public static class SwaggerServiceCollectionExtensions
    {
        private const string SecurityDefinitionName = "Bearer";
        public static IServiceCollection AddSwagger(this IServiceCollection services, VersionInfo version)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Social Media API",
                    Version = version.ProductVersion,
                    Description = "An API used to faciliatate interactions with the Social Media Portfolio application.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });

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

                var apiXmlDocumentPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                var domainXmlDocumentPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{typeof(PostId).Assembly.GetName().Name}.xml");

                options.IncludeXmlComments(apiXmlDocumentPath);

                // flatten example schema IDs
                options.SchemaFilter<FlattenIdSchemaFilter>();
                options.OperationFilter<FlattenIdParameterOperationFilter>();

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
