using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class SynchronizeUserTests
    {
        [Fact]
        public async Task Handle_CallsSynchronizerAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var synchronizer = new Mock<IUserSynchronizer>();
            synchronizer.Setup(s => s.SyncUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new SynchronizeUserHandler(synchronizer.Object);

            var request = new SynchronizeUser
            {
                UserId = user.UserId,
                Name = user.Name
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(user);

            synchronizer.Verify(s => s.SyncUser(
                It.Is<User>(s => s == user),
                It.IsAny<CancellationToken>()));
        }
    }
}
