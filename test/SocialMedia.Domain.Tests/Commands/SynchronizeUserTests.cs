using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class SynchronizeUserTests
    {
        private readonly SynchronizeUserHandler handler;
        private readonly Mock<IUserSynchronizer> userSynchronizer;
        private readonly UserBuilder userBuilder = new();

        public SynchronizeUserTests()
        {
            userSynchronizer = new Mock<IUserSynchronizer>();
            handler = new SynchronizeUserHandler(userSynchronizer.Object);
        }

        [Fact]
        public async Task Handle_CallsSynchronizerAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userSynchronizer.Setup(s => s.SyncUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new SynchronizeUser { UserId = user.Id, Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userSynchronizer.Verify(s => s.SyncUser(
                It.Is<User>(s => s == user),
                cancellationToken));
        }
    }
}
