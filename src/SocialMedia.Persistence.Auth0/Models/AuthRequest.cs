using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record AuthRequest
    {
        [JsonPropertyName("client_id")]
        public string? ClientId { get; init; }

        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; init; }

        [JsonPropertyName("audience")]
        public string? Audience { get; init; }

        [JsonPropertyName("grant_type")]
        public string? GrantType { get; init; }
    }
}
