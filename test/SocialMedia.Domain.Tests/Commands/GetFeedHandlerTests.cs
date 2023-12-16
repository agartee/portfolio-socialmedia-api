using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetFeedHandlerTests
    {
        private readonly GetFeedHandler handler;
        private readonly Mock<IPostRepository> postRepository;
        private readonly PostBuilder postBuilder = new();

        public GetFeedHandlerTests()
        {
            postRepository = new Mock<IPostRepository>();
            handler = new GetFeedHandler(postRepository.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsFeed()
        {
            var feed = new[] { postBuilder.CreatePost().ToPostInfo() };

            postRepository.Setup(r => r.GetAllPosts(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feed);

            var request = new GetFeed();
            var cancellationToken = CancellationToken.None;

            var results = await handler.Handle(request, cancellationToken);

            results.Should().BeEquivalentTo(feed);

            postRepository.Verify(repo => repo.GetAllPosts(cancellationToken));
        }
    }
}
