using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetFeedTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsFeed()
        {
            var feed = new[]
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    UserId = "id",
                    Text = "text",
                    Created = DateTime.UtcNow
                }
            };

            var repository = new Mock<IPostRepository>();
            repository.Setup(r => r.GetAllPosts(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feed);

            var handler = new GetFeedHandler(repository.Object);

            var request = new GetFeed();

            var results = await handler.Handle(request, CancellationToken.None);

            results.Should().BeEquivalentTo(feed);

            repository.Verify(repo => repo.GetAllPosts(
                It.IsAny<CancellationToken>()));
        }
    }
}
