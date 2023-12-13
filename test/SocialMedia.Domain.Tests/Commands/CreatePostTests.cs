using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class CreatePostTests
    {
        private readonly CreatePostHandler handler;
        private readonly Mock<IPostRepository> postRepository;
        private readonly PostBuilder postBuilder = new();

        public CreatePostTests()
        {
            postRepository = new Mock<IPostRepository>();
            handler = new CreatePostHandler(postRepository.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsPost()
        {
            var post = postBuilder.CreatePost();

            postRepository.Setup(r => r.CreatePost(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(post.ToPostInfo());

            var request = new CreatePost { UserId = post.Author!.Id!, Text = post.Text! };
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
