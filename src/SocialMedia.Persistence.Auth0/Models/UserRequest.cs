using System.Text.Json.Serialization;

namespace SocialMedia.Persistence.Auth0.Models
{
    public record UserRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; init; }

        [JsonPropertyName("email")]
        public string? Email { get; init; }
    }
}
