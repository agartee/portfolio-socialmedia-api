using System.Diagnostics;

namespace SocialMedia.WebAPI.Configuration
{
    public record VersionInfo
    {
        public required string AssemblyVersion { get; init; }
        public required string ProductVersion { get; init; }

        public static VersionInfo NewVersionInfo<TTypeInTargetAssembly>()
        {
            var assemblyFile = FileVersionInfo.GetVersionInfo(
                typeof(TTypeInTargetAssembly).Assembly.Location);

            return new VersionInfo
            {
                AssemblyVersion = assemblyFile.FileVersion ?? "n/a",
                ProductVersion = assemblyFile.ProductVersion ?? "n/a"
            };
        }
    }
}
