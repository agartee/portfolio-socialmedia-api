using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.TestUtilities.Builders;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserBuilder userBuilder = new();

        [Fact]
        public async Task Get_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value),
                }, "TestAuthentication"))
            };

            var command = new GetCurrentUser();

            var result = await controller.Get(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<GetCurrentUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<UpdateCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value),
                }, "TestAuthentication"))
            };

            var command = new UpdateCurrentUser { Name = user.Name };

            var result = await controller.Update(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<UpdateCurrentUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Synchronize_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<SynchronizeCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var controller = new WebAPI.Controllers.UserController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value),
                }, "TestAuthentication"))
            };

            var command = new SynchronizeCurrentUser { Name = user.Name };

            var result = await controller.Synchronize(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(
                It.Is<SynchronizeCurrentUser>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
