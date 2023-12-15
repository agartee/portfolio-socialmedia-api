using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateCurrentUserTests
    {
        private readonly UpdateCurrentUserHandler handler;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IUserContext> userContext;
        private readonly UserBuilder userBuilder = new();

        public UpdateCurrentUserTests()
        {
            userRepository = new Mock<IUserRepository>();
            userContext = new Mock<IUserContext>();

            handler = new UpdateCurrentUserHandler(userRepository.Object, userContext.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userContext.Setup(x => x.UserId).Returns(user.Id);
            userRepository.Setup(r => r.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new UpdateCurrentUser { Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userRepository.Verify(r => r.UpdateUser(
                It.Is<User>(s => s == user),
                cancellationToken));
        }
    }
}
