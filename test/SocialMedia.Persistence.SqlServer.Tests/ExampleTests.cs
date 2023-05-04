using FluentAssertions;

namespace SocialMedia.Persistence.SqlServer.Tests
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
