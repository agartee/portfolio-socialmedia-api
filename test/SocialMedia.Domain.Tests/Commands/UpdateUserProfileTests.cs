using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateUserProfileTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUserProfile()
        {
            var userProfile = new UserProfile
            {
                Id = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "me@here.com"
            };

            var repository = new Mock<IUserProfileRepository>();
            repository.Setup(r => r.UpdateUserProfile(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new UpdateUserProfileHandler(repository.Object);

            var request = new UpdateUserProfile
            {
                Id = userProfile.Id,
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.UpdateUserProfile(
                It.Is<UserProfile>(s => s == userProfile),
                It.IsAny<CancellationToken>()));
        }
    }
}
