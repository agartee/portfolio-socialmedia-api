using FluentAssertions;
using SocialMedia.Persistence.SqlServer.Extensions;
using SocialMedia.TestUtilities;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Persistence.SqlServer.Tests.Extensions
{
    public class UserExtensionsTests
    {
        private readonly UserBuilder userBuilder = new();

        [Fact, UseMappingContextScope]
        public void ToUser_ReturnsExpectedUserInfo()
        {
            var user = userBuilder.CreateUser();
            var userData = user.ToUserData();

            var result = userData.ToUser();

            result.Should().Be(user.ToUser());
        }
    }
}
