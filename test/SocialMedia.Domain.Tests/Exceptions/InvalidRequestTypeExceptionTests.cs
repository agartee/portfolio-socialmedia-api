using FluentAssertions;
using SocialMedia.Domain.Exceptions;

namespace SocialMedia.Domain.Tests.Exceptions
{
    public class InvalidRequestTypeExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var type = typeof(string);
            var exception = new InvalidRequestTypeException(type);

            exception.Message.Should().Be("Type is not a MediatR IRequest: System.String");
        }
    }
}
