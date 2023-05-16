using FluentAssertions;
using SocialMedia.Persistence.Auth0.Exceptions;

namespace SocialMedia.Persistence.Auth0.Tests.Exceptions
{
    public class CannotDeserializeResponseExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var url = "https://test.com";
            var type = typeof(string);
            var exception = new CannotDeserializeResponseException(url, type);

            exception.Message.Should().Be("Unable to deserialize HTTP response message from https://test.com to System.String.");
        }
    }
}
