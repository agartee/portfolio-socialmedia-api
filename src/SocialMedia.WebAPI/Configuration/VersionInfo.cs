namespace SocialMedia.WebAPI.Configuration
{
    public record VersionInfo
    {
        public required string Version { get; init; }
        public required string Build {  get; init; }

        public static VersionInfo Default => new VersionInfo
        {
            Version = "n/a",
            Build = "n/a"
        };
    }
}
