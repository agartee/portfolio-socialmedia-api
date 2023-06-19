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
    public class BasicUserProfileControllerTests
    {
        [Fact]
        public async Task Get_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new BasicUserProfile
            {
                UserId = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "email",
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetBasicUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            }, "TestAuthentication"));

            var controller = new WebAPI.Controllers.BasicUserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new GetBasicUserProfile
            {
                UserId = userProfile.UserId
            };

            var result = await controller.Get(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<GetBasicUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Patch_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new BasicUserProfile
            {
                UserId = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "email",
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<UpdateBasicUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            }, "TestAuthentication"));

            var controller = new WebAPI.Controllers.BasicUserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new UpdateBasicUserProfile
            {
                UserId = userProfile.UserId,
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email,
            };

            var result = await controller.Update(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<UpdateBasicUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
