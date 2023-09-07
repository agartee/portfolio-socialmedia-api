using CommandLine;

namespace SocialMedia.WebAPI.Configuration
{
    public static class ParserExtensions
    {
        public static string GetConnectionStringName(this Parser parser, string[] args)
        {
            var dbConnectionString = "database";

            parser.ParseArguments<CommandLineArgs>(args)
                .WithParsed(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.DbConnectionStringName))
                    {
                        dbConnectionString = x.DbConnectionStringName;
                    }
                });

            return dbConnectionString;
        }
    }
}
