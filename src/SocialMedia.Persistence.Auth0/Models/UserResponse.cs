using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record UserResponse
    {
        [JsonPropertyName("user_id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; init; }

        [JsonPropertyName("email")]
        public string? Email { get; init; }
    }
}
