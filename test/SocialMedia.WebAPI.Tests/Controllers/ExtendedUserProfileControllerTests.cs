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
    public class ExtendedUserProfileControllerTests
    {
        [Fact]
        public async Task Get_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new ExtendedUserProfile
            {
                UserId = "id",
                DisplayName = "display name"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            }, "TestAuthentication"));

            var controller = new ExtendedUserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new GetExtendedUserProfile
            {
                UserId = userProfile.UserId
            };

            var result = await controller.Get(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<GetExtendedUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Patch_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new ExtendedUserProfile
            {
                UserId = "id",
                DisplayName = "display name"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<UpdateExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            }, "TestAuthentication"));

            var controller = new WebAPI.Controllers.ExtendedUserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new UpdateExtendedUserProfile
            {
                UserId = userProfile.UserId,
                DisplayName = userProfile.DisplayName
            };

            var result = await controller.Update(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<UpdateExtendedUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
