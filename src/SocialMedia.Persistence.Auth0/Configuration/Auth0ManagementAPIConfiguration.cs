namespace SocialMedia.Persistence.Auth0.Configuration
{
    public class Auth0ManagementAPIConfiguration
    {
        public string? Audience { get; init; }
        public string? ClientId { get; init; }
        public string? ClientSecret { get; init; }

        public static Auth0ManagementAPIConfiguration Empty()
        {
            return new Auth0ManagementAPIConfiguration
            {
                Audience = "",
                ClientId = "",
                ClientSecret = "",
            };
        }
    }
}
