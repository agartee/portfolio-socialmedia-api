using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class SynchronizeCurrentUserTests
    {
        private readonly SynchronizeCurrentUserHandler handler;
        private readonly Mock<IUserSynchronizer> userSynchronizer;
        private readonly Mock<IUserContext> userContext;
        private readonly UserBuilder userBuilder = new();

        public SynchronizeCurrentUserTests()
        {
            userSynchronizer = new Mock<IUserSynchronizer>();
            userContext = new Mock<IUserContext>();

            handler = new SynchronizeCurrentUserHandler(userSynchronizer.Object, userContext.Object);
        }

        [Fact]
        public async Task Handle_CallsSynchronizerAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userContext.Setup(x => x.UserId).Returns(user.Id);
            userSynchronizer.Setup(s => s.SyncUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new SynchronizeCurrentUser { Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userSynchronizer.Verify(s => s.SyncUser(
                It.Is<User>(s => s == user),
                cancellationToken));
        }
    }
}
