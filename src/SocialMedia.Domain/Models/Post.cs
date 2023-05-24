namespace SocialMedia.Domain.Models
{
    public record Post
    {
        public required Guid Id { get; init; }
        public required string UserId { get; init; }
        public required string Text { get; set; }
        public required DateTime Created { get; init; }
    }
}
