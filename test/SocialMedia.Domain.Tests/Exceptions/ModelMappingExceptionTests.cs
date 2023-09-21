using FluentAssertions;
using SocialMedia.Domain.Exceptions;

namespace SocialMedia.Domain.Tests.Exceptions
{
    public class ModelMappingExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var exception = new ModelMappingException<Thing, OtherThing>("Something bad happened.");

            exception.Message.Should().Be("Unable to map Thing to OtherThing. Something bad happened.");
        }

        public record Thing
        {
        }

        public record OtherThing
        {
        }
    }
}
