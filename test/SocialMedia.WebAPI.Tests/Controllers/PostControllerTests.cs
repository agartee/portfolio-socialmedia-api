using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly PostController controller;
        private readonly Mock<IMediator> mediator = new();
        private readonly PostBuilder postBuilder = new();

        public PostControllerTests()
        {
            controller = new PostController(mediator.Object);
        }

        //[Fact]
        //public async Task Create_SubmitsCommandAndReturnsResult()
        //{
        //    var post = postBuilder.CreatePost().ToPostInfo();

        //    mediator.Setup(m => m.Send(It.IsAny<CreatePost>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(post);

        //    var command = new CreatePost { Text = post.Text };
        //    var cancellationToken = CancellationToken.None;

        //    var result = await controller.Create(command, cancellationToken);

        //    result.Should().BeOfType<OkObjectResult>();
        //    result.As<OkObjectResult>().Value.Should().Be(post);

        //    mediator.Verify(m => m.Send(command, cancellationToken));
        //}

        [Fact]
        public async Task Demand_SubmitsCommandAndReturnsResult()
        {
            var post = postBuilder.CreatePost().ToPostInfo();

            mediator.Setup(m => m.Send(It.IsAny<DemandPost>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(post);

            var command = new DemandPost { Id = post.Id! };
            var cancellationToken = CancellationToken.None;

            var result = await controller.Demand(post.Id!, cancellationToken);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(post);

            mediator.Verify(m => m.Send(command, cancellationToken));
        }
    }
}
