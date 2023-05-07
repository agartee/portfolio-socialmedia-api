using CommandLine;
using FluentAssertions;
using SocialMedia.Domain.Exceptions;

namespace SocialMedia.Domain.Tests.Exceptions
{
    public class CommandLineParsingExceptionTests
    {
        [Fact]
        public void Message_ReturnsExpectedString()
        {
            var commandArg = "bad";
            var parserResult = Parser.Default.ParseArguments(new[] { commandArg }, new[] { typeof(TestCommand) });

            var exception = new CommandLineParsingException(parserResult);

            exception.Message.Should().Contain("Unable to execute command.");
            exception.Message.Should().Contain(commandArg);
        }

        [Verb("test")]
        public class TestCommand
        {
        }
    }
}
