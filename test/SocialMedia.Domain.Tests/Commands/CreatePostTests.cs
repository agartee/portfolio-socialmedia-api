using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class CreatePostTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsPost()
        {
            var repository = new Mock<IPostRepository>();
            var handler = new CreatePostHandler(repository.Object);

            var request = new CreatePost
            {
                UserId = "id",
                Text = "text"
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Id.Should().NotBe(Guid.Empty);
            result.Author.Should().Be(request.UserId);
            result.Text.Should().Be(request.Text);
            result.Created.Should()
                .BeOnOrAfter(DateTime.UtcNow.AddMinutes(-10)) // for debugger safety
                .And
                .BeOnOrBefore(DateTime.UtcNow);

            repository.Verify(repo => repo.CreatePost(
                It.Is<Post>(p => p.Equals(result)),
                It.IsAny<CancellationToken>()));
        }
    }
}
