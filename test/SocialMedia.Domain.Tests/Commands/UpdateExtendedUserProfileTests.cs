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
        public async Task Handle_CallsRepositoryAndReturnsUserProfile()
        {
            var userProfile = new ExtendedUserProfile
            {
                UserId = "id",
                DisplayName = "display name"
            };

            var repository = new Mock<IExtendedUserProfileRepository>();
            repository.Setup(r => r.UpdateExtendedUserProfile(It.IsAny<ExtendedUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new UpdateExtendedUserProfileHandler(repository.Object);

            var request = new UpdateExtendedUserProfile
            {
                UserId = userProfile.UserId,
                DisplayName = "display name"
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.UpdateExtendedUserProfile(
                It.Is<ExtendedUserProfile>(s => s == userProfile),
                It.IsAny<CancellationToken>()));
        }
    }
}
