using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class CreatePostHandlerTests
    {
        private readonly CreatePostHandler handler;
        private readonly Mock<IPostRepository> postRepository;
        private readonly Mock<IUserContext> userContext;
        private readonly PostBuilder postBuilder = new();

        public CreatePostHandlerTests()
        {
            postRepository = new Mock<IPostRepository>();
            userContext = new Mock<IUserContext>();

            handler = new CreatePostHandler(postRepository.Object, userContext.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsPost()
        {
            var post = postBuilder.CreatePost();

            userContext.Setup(x => x.UserId).Returns(post.Author!.Id!);
            postRepository.Setup(r => r.CreatePost(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(post.ToPostInfo());

            var request = new CreatePost { Text = post.Text! };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(post.ToPostInfo());

            postRepository.Verify(repo => repo.CreatePost(
                It.Is<Post>(p =>
                    p.AuthorUserId == post.Author.Id
                    && p.Text == post.Text),
                cancellationToken));
        }
    }
}
