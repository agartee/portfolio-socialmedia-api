using System.Text.Json;

namespace SocialMedia.WebAPI.Configuration
{
    public static class VersionInfoServiceCollectionExtensions
    {
        public static IServiceCollection AddVersionInfo(this IServiceCollection services)
        {
            var jsonFilePath = Path.Combine(AppContext.BaseDirectory, "version.json");

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var appConfig = JsonSerializer.Deserialize<VersionInfo>(File.ReadAllText(jsonFilePath), jsonOptions) ?? VersionInfo.Default;

            services.AddSingleton(appConfig);

            return services;
        }
    }
}
