namespace SocialMedia.Domain.Models
{
    public record PostInfo
    {
        public required Guid Id { get; init; }
        public required string Author { get; init; }
        public required string Text { get; set; }
        public required DateTime Created { get; init; }
    }
}
