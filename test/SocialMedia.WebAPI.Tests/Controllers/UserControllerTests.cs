using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserController controller;
        private readonly Mock<IMediator> mediator = new();
        private readonly UserBuilder userBuilder = new();

        public UserControllerTests()
        {
            controller = new UserController(mediator.Object);
        }

        [Fact]
        public async Task Get_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            mediator.Setup(m => m.Send(It.IsAny<GetCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var cancellationToken = CancellationToken.None;
            var result = await controller.Get(cancellationToken);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(It.IsAny<GetCurrentUser>(), cancellationToken));
        }

        [Fact]
        public async Task Update_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            mediator.Setup(m => m.Send(It.IsAny<UpdateCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var command = new UpdateCurrentUser { Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await controller.Update(command, cancellationToken);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(command, cancellationToken));
        }

        [Fact]
        public async Task Synchronize_SubmitsCommandAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            mediator.Setup(m => m.Send(It.IsAny<SynchronizeCurrentUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var command = new SynchronizeCurrentUser { Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await controller.Synchronize(command, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(user);

            mediator.Verify(m => m.Send(command, cancellationToken));
        }
    }
}
