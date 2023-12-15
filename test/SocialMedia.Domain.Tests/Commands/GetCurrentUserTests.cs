using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetCurrentUserTests
    {
        private readonly GetCurrentUserHandler handler;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IUserContext> userContext;
        private readonly UserBuilder userBuilder = new();

        public GetCurrentUserTests()
        {
            userRepository = new Mock<IUserRepository>();
            userContext = new Mock<IUserContext>();

            handler = new GetCurrentUserHandler(userRepository.Object, userContext.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userContext.Setup(x => x.UserId).Returns(user.Id);
            userRepository.Setup(r => r.GetUser(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new GetCurrentUser();
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userRepository.Verify(m => m.GetUser(
                It.Is<UserId>(s => s == user.Id),
                cancellationToken));
        }
    }
}
