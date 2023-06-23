using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateUserTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name"
            };

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new UpdateUserHandler(repository.Object);

            var request = new UpdateUser
            {
                UserId = user.UserId,
                Name = user.Name
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(user);

            repository.Verify(r => r.UpdateUser(
                It.Is<User>(s => s == user),
                It.IsAny<CancellationToken>()));
        }
    }
}
