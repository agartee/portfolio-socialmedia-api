using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record AuthToken
    {
        [JsonPropertyName("token_type")]
        public required string TokenType { get; init; }
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }
    }
}
