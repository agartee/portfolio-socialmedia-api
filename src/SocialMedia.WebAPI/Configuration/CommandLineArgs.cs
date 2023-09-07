using CommandLine;

namespace SocialMedia.WebAPI.Configuration
{
    public class CommandLineArgs
    {
        [Option('d', "DbConnectionStringName",
            Required = false,
            HelpText = "Name of the database connection string to use (defined in environment variables, user secrets, etc.)")]
        public string? DbConnectionStringName { get; set; }
    }
}
