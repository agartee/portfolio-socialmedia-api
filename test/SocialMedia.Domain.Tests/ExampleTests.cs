using FluentAssertions;

namespace SocialMedia.Domain.Tests
{
    public class ExampleTests
    {
        [Fact]
        public void Exists_ReturnsTrue()
        {
            new Example().Exists().Should().BeTrue();
        }
    }
}
