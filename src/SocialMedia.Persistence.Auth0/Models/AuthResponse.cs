using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record AuthResponse
    {
        [JsonPropertyName("token_type")]
        public required string TokenType { get; init; }

        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("scope")]
        public required string Scope { get; init; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }
    }
}
