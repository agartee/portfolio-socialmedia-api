using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class SynchronizeUserProfileTests
    {
        [Fact]
        public async Task Handle_CallsSynchronizerAndReturnsUserProfile()
        {
            var userProfile = new UserProfile
            {
                UserId = "id",
                Name = "name",
                Email = "me@here.com"
            };

            var synchronizer = new Mock<IUserProfileRepository>();
            synchronizer.Setup(s => s.UpdateUserProfile(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new SynchronizeUserProfileHandler(synchronizer.Object);

            var request = new SynchronizeUserProfile
            {
                UserId = userProfile.UserId,
                Name = userProfile.Name,
                Email = userProfile.Email
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            synchronizer.Verify(s => s.UpdateUserProfile(
                It.Is<UserProfile>(s => s == userProfile),
                It.IsAny<CancellationToken>()));
        }
    }
}
