using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Controllers;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class FeedControllerTests
    {
        [Fact]
        public async Task Get_SubmitsCommandAndReturnsResult()
        {
            var feed = new[]
            {
                new PostInfo
                {
                    Id = Guid.NewGuid(),
                    Author = "User 1",
                    Text = "text",
                    Created = DateTime.UtcNow
                }
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetFeed>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(feed);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "some-id"),
            }, "TestAuthentication"));

            var controller = new FeedController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new GetFeed();

            var result = await controller.Get(CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(feed);

            mediator.Verify(m => m.Send(
                It.IsAny<GetFeed>(),
                It.IsAny<CancellationToken>()));
        }
    }
}
