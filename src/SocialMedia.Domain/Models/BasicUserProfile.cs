namespace SocialMedia.Domain.Models
{
    public record BasicUserProfile
    {
        public required string UserId { get; init; }
        public string? Name { get; init; }
        public string? Nickname { get; init; }
        public string? Email { get; init; }
    }
}
