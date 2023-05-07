using CommandLine;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Services;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class CliControllerTests
    {
        [Fact]
        public async Task Run_WhenRequestCompletesSuccessfully_ReturnsResultFromRequestHandler()
        {
            const string expectedResult = "ok!";

            var requestBuilder = new Mock<ICliRequestBuilder>();
            requestBuilder.Setup(rb => rb.BuildRequest(It.IsAny<string>()))
                .Returns(new TestCommand());

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<IBaseRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var controller = new CliController(requestBuilder.Object, mediator.Object);

            var result = await controller.Run("some command", CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        }

        [Fact]
        public async Task Run_WhenCommandTextFailsToParse_ReturnsBadRequest()
        {
            var requestBuilder = new Mock<ICliRequestBuilder>();
            requestBuilder.Setup(rb => rb.BuildRequest(It.IsAny<string>()))
                .Throws(new CommandLineParsingException(
                    new Parser().ParseArguments(Enumerable.Empty<string>(), typeof(TestCommand))));

            var mediator = new Mock<IMediator>();

            var controller = new CliController(requestBuilder.Object, mediator.Object);

            var result = await controller.Run("some failing command", CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        public class TestCommand : IRequest<string>
        {
        }
    }
}
