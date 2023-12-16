using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class FeedControllerTests
    {
        private readonly FeedController controller;
        private readonly Mock<IMediator> mediator = new();
        private readonly PostBuilder postBuilder = new();

        public FeedControllerTests()
        {
            controller = new FeedController(mediator.Object);
        }

        [Fact]
        public async Task Get_SubmitsCommandAndReturnsResult()
        {
            var feed = new[]
            {
                postBuilder.CreatePost().ToPostInfo()
            };

            mediator.Setup(m => m.Send(It.IsAny<GetFeed>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(feed);

            var command = new GetFeed();
            var cancellationToken = CancellationToken.None;

            var result = await controller.Get(cancellationToken);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(feed);

            mediator.Verify(m => m.Send(
                It.IsAny<GetFeed>(),
                cancellationToken));
        }
    }
}
