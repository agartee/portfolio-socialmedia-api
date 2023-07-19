using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record UserResponse
    {
        [JsonPropertyName("user_id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}
