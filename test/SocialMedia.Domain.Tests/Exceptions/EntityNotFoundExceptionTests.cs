using FluentAssertions;
using SocialMedia.Domain.Exceptions;

namespace SocialMedia.Domain.Tests.Exceptions
{
    public class EntityNotFoundExceptionTests
    {
        [Fact]
        public void Message_WhenIdDefined_ReturnsExpectedString()
        {
            var id = "id";
            var type = typeof(TestEntity);

            var exception = new EntityNotFoundException(type, id);

            exception.Message.Should().Be("Unable to find TestEntity with identifier: id");
        }

        [Fact]
        public void Message_WhenIdNotDefined_ReturnsExpectedString()
        {
            var type = typeof(TestEntity);

            var exception = new EntityNotFoundException(type);

            exception.Message.Should().Be("Unable to find TestEntity with identifier: unknown");
        }

        public class TestEntity
        {
        }
    }
}
