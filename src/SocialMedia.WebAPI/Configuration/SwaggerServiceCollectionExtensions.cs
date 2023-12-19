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

                var apiXmlDocumentPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                var domainXmlDocumentPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{typeof(PostId).Assembly.GetName().Name}.xml");

                options.IncludeXmlComments(apiXmlDocumentPath);

                options.OperationFilter<IdPathParameterFilter>(XDocument.Load(apiXmlDocumentPath));
                options.OperationFilter<RequestQueryParameterFilter>(XDocument.Load(domainXmlDocumentPath));
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
