namespace SocialMedia.Domain.Models
{
    public record User
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }
}
