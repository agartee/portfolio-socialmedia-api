using FluentAssertions;
using SocialMedia.Persistence.Auth0.Exceptions;

namespace SocialMedia.Persistence.Auth0.Tests.Exceptions
{
    public class AuthenticationFailedExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var exception = new AuthenticationFailedException();

            exception.Message.Should().Be("Unable to login as client.");
        }
    }
}
