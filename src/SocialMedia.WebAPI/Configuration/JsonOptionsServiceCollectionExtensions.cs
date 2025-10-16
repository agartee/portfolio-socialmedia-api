using Microsoft.AspNetCore.Http.Json;
using SocialMedia.WebAPI.JsonConverters;

namespace SocialMedia.WebAPI.Configuration
{
    public static class JsonOptionsServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonOptions(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new IdJsonConverter());
            });

            return services;
        }
    }
}
