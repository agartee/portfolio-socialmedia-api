using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateUserTests
    {
        private readonly UpdateUserHandler handler;
        private readonly Mock<IUserRepository> userRepository;
        private readonly UserBuilder userBuilder = new();

        public UpdateUserTests()
        {
            userRepository = new Mock<IUserRepository>();
            handler = new UpdateUserHandler(userRepository.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userRepository.Setup(r => r.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new UpdateUser { UserId = user.Id, Name = user.Name };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userRepository.Verify(r => r.UpdateUser(
                It.Is<User>(s => s == user),
                cancellationToken));
        }
    }
}
