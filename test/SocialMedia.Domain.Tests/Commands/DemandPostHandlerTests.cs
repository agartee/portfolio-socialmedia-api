using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class DemandPostHandlerTests
    {
        private readonly DemandPostHandler handler;
        private readonly Mock<IPostRepository> postRepository;
        private readonly PostBuilder postBuilder = new();

        public DemandPostHandlerTests()
        {
            postRepository = new Mock<IPostRepository>();
            handler = new DemandPostHandler(postRepository.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsPost()
        {
            var post = postBuilder.CreatePost().ToPostInfo();

            postRepository.Setup(r => r.DemandPost(It.IsAny<PostId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(post);

            var request = new DemandPost { Id = post.Id };
            var cancellationToken = CancellationToken.None;

            var results = await handler.Handle(request, cancellationToken);

            results.Should().BeEquivalentTo(post);

            postRepository.Verify(repo => repo.DemandPost(post.Id, cancellationToken));
        }
    }
}
