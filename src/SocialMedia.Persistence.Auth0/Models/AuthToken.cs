namespace SocialMedia.Persistence.Auth0.Models
{
    public record AuthToken
    {
        public required string TokenType { get; init; }
        public required string AccessToken { get; init; }
    }
}
