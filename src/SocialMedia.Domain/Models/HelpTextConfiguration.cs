namespace SocialMedia.Domain.Models
{
    public class HelpTextConfiguration
    {
        private static readonly string defaultHeading = "Commissioner API";
        private static readonly string defaultCopyright = $"Copyright © {DateTime.Now.Year} Adam Gartee";

        public HelpTextConfiguration(string heading, string copyright, params Type[] types)
        {
            Heading = heading;
            Copyright = copyright;
            Types = types;
        }

        public HelpTextConfiguration(params Type[] types)
            : this(defaultHeading, defaultCopyright, types)
        {
        }

        public Type[] Types { get; }
        public string Heading { get; }
        public string Copyright { get; }
    }
}
