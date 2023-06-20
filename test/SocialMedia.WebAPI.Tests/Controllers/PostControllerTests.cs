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
    public class PostControllerTests
    {
        [Fact]
        public async Task Create_SubmitsCommandAndReturnsResult()
        {
            var userId = "123";

            var post = new PostInfo
            {
                Id = Guid.NewGuid(),
                Author = "User 1",
                Text = "text",
                Created = DateTime.UtcNow
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePost>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(post);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, "TestAuthentication"));

            var controller = new PostController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new CreatePost
            {
                UserId = userId,
                Text = post.Text
            };

            var result = await controller.Create(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(post);

            mediator.Verify(m => m.Send(
                It.Is<CreatePost>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
