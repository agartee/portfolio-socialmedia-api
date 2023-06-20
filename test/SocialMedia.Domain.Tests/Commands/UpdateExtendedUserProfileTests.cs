using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateExtendedUserProfileTests
    {
        [Fact]
        public async Task Handle_WhenDisplayNameProvidedAndUserProfileExists_UpdatesAndReturnsUserProfile()
        {
            var userId = "123";

            var existingUserProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "original display name"
            };

            var updatedUserProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "updated display name"
            };

            var repository = new Mock<IExtendedUserProfileRepository>();
            repository.Setup(r => r.GetExtendedUserProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUserProfile);
            repository.Setup(r => r.UpdateExtendedUserProfile(It.IsAny<ExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUserProfile);

            var handler = new UpdateExtendedUserProfileHandler(repository.Object);

            var request = new UpdateExtendedUserProfile
            {
                UserId = updatedUserProfile.UserId,
                DisplayName = updatedUserProfile.DisplayName
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(updatedUserProfile);

            repository.Verify(m => m.UpdateExtendedUserProfile(
                It.Is<ExtendedUserProfile>(s => s == updatedUserProfile),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Handle_WhenDisplayNameNotProvidedAndUserProfileExists_DoesNotUpdateDisplayName()
        {
            var userId = "123";

            var existingUserProfile = new ExtendedUserProfile
            {
                UserId = userId,
                DisplayName = "original display name"
            };

            var repository = new Mock<IExtendedUserProfileRepository>();
            repository.Setup(r => r.GetExtendedUserProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUserProfile);
            repository.Setup(r => r.UpdateExtendedUserProfile(It.IsAny<ExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUserProfile);

            var handler = new UpdateExtendedUserProfileHandler(repository.Object);

            // note: no DisplayName
            var request = new UpdateExtendedUserProfile
            {
                UserId = userId
            };

            var result = await handler.Handle(request, CancellationToken.None);

            repository.Verify(m => m.UpdateExtendedUserProfile(
                It.Is<ExtendedUserProfile>(s => s == existingUserProfile), // still original...
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Handle_WhenUserProfileNotExists_CreatesAndReturnsUserProfile()
        {
            var newUserProfile = new ExtendedUserProfile
            {
                UserId = "123",
                DisplayName = "display name"
            };

            var repository = new Mock<IExtendedUserProfileRepository>();
            repository.Setup(r => r.CreateExtendedUserProfile(It.IsAny<ExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newUserProfile);

            var handler = new UpdateExtendedUserProfileHandler(repository.Object);

            var request = new UpdateExtendedUserProfile
            {
                UserId = newUserProfile.UserId,
                DisplayName = newUserProfile.DisplayName
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(newUserProfile);

            repository.Verify(m => m.CreateExtendedUserProfile(
                It.Is<ExtendedUserProfile>(s => s == newUserProfile),
                It.IsAny<CancellationToken>()));
        }
    }
}
