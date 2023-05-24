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
    public class UserProfileControllerTests
    {
        [Fact]
        public async Task Get_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new UserProfile
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "email",
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id),
            }, "TestAuthentication"));

            var controller = new UserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new GetUserProfile
            {
                UserId = userProfile.Id
            };

            var result = await controller.Get(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<GetUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_SubmitsCommandAndReturnsResult()
        {
            var userProfile = new UserProfile
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "email",
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<UpdateUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id),
            }, "TestAuthentication"));

            var controller = new UserProfileController(mediator.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var command = new UpdateUserProfile
            {
                UserId = userProfile.Id,
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email,
            };

            var result = await controller.Patch(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(userProfile);

            mediator.Verify(m => m.Send(
                It.Is<UpdateUserProfile>(r => r == command),
                It.IsAny<CancellationToken>()));
        }
    }
}
