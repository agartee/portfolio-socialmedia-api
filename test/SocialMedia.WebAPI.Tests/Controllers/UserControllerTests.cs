using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public async Task Get_SubmitsCommandAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                }, "TestAuthentication"))
            };

            var command = new GetUser
            {
                UserId = user.UserId
            };

            var result = await controller.Get(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<GetUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_SubmitsCommandAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<UpdateUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                }, "TestAuthentication"))
            };

            var command = new UpdateUser
            {
                UserId = user.UserId,
                Name = user.Name
            };

            var result = await controller.Update(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<UpdateUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Synchronize_SubmitsCommandAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<SynchronizeUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                }, "TestAuthentication"))
            };

            var command = new SynchronizeUser
            {
                UserId = user.UserId,
                Name = user.Name
            };

            var result = await controller.Synchronize(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<SynchronizeUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
