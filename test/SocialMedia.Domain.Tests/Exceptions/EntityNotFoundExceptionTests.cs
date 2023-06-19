using FluentAssertions;
using SocialMedia.Domain.Exceptions;

namespace SocialMedia.Domain.Tests.Exceptions
{
    public class EntityNotFoundExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var typeName = "Thing";
            var id = 1;

            var exception = new EntityNotFoundException(typeName, id);

            exception.Message.Should().Be("Unable to find Thing with identifier: 1");
        }
    }
}
