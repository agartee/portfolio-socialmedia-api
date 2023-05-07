using CommandLine;
using FluentAssertions;
using MediatR;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Services
{
    public class CliRequestBuilderTests
    {
        [Fact]
        public void BuildRequest_WithSingleWordVerbAndNoOptions_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(SingleWordTestCommandWithNoOptions));
            var commandText = "test";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.Should().BeOfType<SingleWordTestCommandWithNoOptions>();
        }

        [Fact]
        public void BuildRequest_WithMultiWordVerbAndNoOptions_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(MultiWordTestCommandWithNoOptions));
            var commandText = "thing test";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.Should().BeOfType<MultiWordTestCommandWithNoOptions>();
        }

        [Fact]
        public void BuildRequest_WithSingleWordVerbAndSingleOption_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = "test --foo hello";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.As<TestCommandWithSingleOption>().Foo.Should().Be("hello");
        }

        [Fact]
        public void BuildRequest_WithOptionValueWrappedInQuotes_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = "test --foo \"hello there\"";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.As<TestCommandWithSingleOption>().Foo.Should().Be("hello there");
        }

        [Fact]
        public void BuildRequest_WithOptionValueContainingEscapedDoubleQuotes_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = """
                test --foo "\"hello there\""
                """;

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.As<TestCommandWithSingleOption>().Foo.Should().Be("\"hello there\"");
        }

        [Fact]
        public void BuildRequest_WithSingleWordVerbAndMultipleOptions_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithMultipleOptions));
            var commandText = "test --foo hello --bar there";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.As<TestCommandWithMultipleOptions>().Foo.Should().Be("hello");
            result.As<TestCommandWithMultipleOptions>().Bar.Should().Be("there");
        }

        [Fact]
        public void BuildRequest_WithMultiWordVerbAndMultipleOptions_ReturnsRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(MultiWordTestCommandWithMultipleOptions));
            var commandText = "subject test --foo hello --bar there";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.As<MultiWordTestCommandWithMultipleOptions>().Foo.Should().Be("hello");
            result.As<MultiWordTestCommandWithMultipleOptions>().Bar.Should().Be("there");
        }

        [Fact]
        public void BuildRequest_WhenRequestTypesIsEmpty_Throws()
        {
            var cliRequestBuilder = new CliRequestBuilder(Array.Empty<Type>());
            var commandText = "test";

            var action = () => cliRequestBuilder.BuildRequest(commandText);
            action.Should().Throw<CommandLineParsingException>();
        }

        [Fact]
        public void BuildRequest_WithNonexistingOptions_Throws()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = "test --baz nope!";

            var action = () => cliRequestBuilder.BuildRequest(commandText);
            action.Should().Throw<CommandLineParsingException>();
        }

        [Fact]
        public void BuildRequest_WithRequestClassThatIsNotAnIRequest_Throws()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandThatIsNotAnIRequest));
            var commandText = "test --text Hello.";

            var action = () => cliRequestBuilder.BuildRequest(commandText);

            var error = action.Should().Throw<InvalidRequestTypeException>()
                .WithMessage($"*{nameof(TestCommandThatIsNotAnIRequest)}*");
        }

        [Fact]
        public void BuildRequest_WithCommandAndHelpArg_ReturnsGetHelpTextRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = "test --help";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.Should().BeOfType<GetHelpText>();
            result.As<GetHelpText>().ParserResult.Should().NotBeNull();
        }

        [Fact]
        public void BuildRequest_WithHelpArgOnly_ReturnsGetHelpTextRequest()
        {
            var cliRequestBuilder = new CliRequestBuilder(typeof(TestCommandWithSingleOption));
            var commandText = "--help";

            var result = cliRequestBuilder.BuildRequest(commandText);

            result.Should().BeOfType<GetHelpText>();
            result.As<GetHelpText>().ParserResult.Should().BeNull();
        }

        [Fact]
        public void TrimSingle_WhenStringDoesNotContainTrimmedChar_ReturnsOriginalString()
        {
            var original = "hello";
            var result = original.TrimSingle('-');

            result.Should().Be(original);
        }

        [Fact]
        public void TrimSingle_WhenStringContainTrimmedChar_ReturnsTrimmedString()
        {
            var original = "--hello--";
            var result = original.TrimSingle('-');

            result.Should().Be("-hello-");
        }

        [Fact]
        public void TrimSingle_WhenStringStartsWithTrimmedChar_ReturnsTrimmedString()
        {
            var original = "--hello";
            var result = original.TrimSingle('-');

            result.Should().Be("-hello");
        }

        [Fact]
        public void TrimSingle_WhenStringEndsWithTrimmedChar_ReturnsTrimmedString()
        {
            var original = "hello--";
            var result = original.TrimSingle('-');

            result.Should().Be("hello-");
        }

        [Verb("test")]
        public class SingleWordTestCommandWithNoOptions : IRequest
        {
        }

        [Verb("thing test")]
        public class MultiWordTestCommandWithNoOptions : IRequest
        {
        }

        [Verb("test")]
        public class TestCommandWithSingleOption : IRequest
        {
            [Option(Required = false)]
            public string? Foo { get; set; }
        }

        [Verb("test")]
        public class TestCommandWithMultipleOptions : IRequest
        {
            [Option(Required = false)]
            public string? Foo { get; set; }

            [Option(Required = false)]
            public string? Bar { get; set; }
        }

        [Verb("subject test")]
        public class MultiWordTestCommandWithMultipleOptions : IRequest
        {
            [Option(Required = false)]
            public string? Foo { get; set; }

            [Option(Required = false)]
            public string? Bar { get; set; }
        }

        [Verb("test")]
        public class TestCommandThatIsNotAnIRequest
        {
            [Option(Required = false)]
            public string? Text { get; set; }
        }
    }
}
