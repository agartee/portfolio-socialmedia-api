using CommandLine;
using FluentAssertions;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetHelpTextTests
    {
        [Fact]
        public async Task Handle_WhenParsedResultHasVerbAttribute_ReturnsHelpTextContainingVerbHelpText()
        {
            var parserResult = Parser.Default.ParseArguments(
                new[] { "test", "--help" },
                new Type[] { typeof(TestCommandWithAttributes) });

            var request = new GetHelpText(parserResult);
            var handler = new GetHelpTextHandler(new HelpTextConfiguration());

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Contain(VERB_HELP_TEXT);
        }

        [Fact]
        public async Task Handle_WhenParsedResultDoesNotHaveVerbAttribute_ReturnsStandardHelpText()
        {
            var parserResult = Parser.Default.ParseArguments(
                new[] { "test", "--help" },
                new Type[] { typeof(TestCommandWithAttributes) });

            var request = new GetHelpText(parserResult);
            var handler = new GetHelpTextHandler(new HelpTextConfiguration());

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().ContainAll("--help");
        }

        [Fact]
        public async Task Handle_WhenParsedResultPropertyHasOptionAttribute_ReturnsHelpTextContainingOptionHelpText()
        {
            var parserResult = Parser.Default.ParseArguments(
                new[] { "test", "--help" },
                new Type[] { typeof(TestCommandWithAttributes) });

            var request = new GetHelpText(parserResult);
            var handler = new GetHelpTextHandler(new HelpTextConfiguration());

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Contain(OPTION_HELP_TEXT);
        }

        [Fact]
        public async Task Handle_WhenParsedResultPropertyDoesNotHaveOptionAttribute_ReturnsGeneralHelpText()
        {
            var parserResult = Parser.Default.ParseArguments(
                new[] { "test", "--help" },
                new Type[] { typeof(TestCommandWithoutAttributes) });

            var request = new GetHelpText(parserResult);
            var handler = new GetHelpTextHandler(new HelpTextConfiguration());

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().NotContain("--text");
        }

        [Fact]
        public async Task Handle_WhenParserResultIsNull_ReturnsGeneralHelpText()
        {
            var request = new GetHelpText();
            var handler = new GetHelpTextHandler(new HelpTextConfiguration(
                typeof(TestCommandWithAttributes)));

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Contain(VERB_HELP_TEXT);
            result.Should().NotContain(OPTION_HELP_TEXT);
        }

        const string VERB_HELP_TEXT = "test: Test stuff.";
        const string OPTION_HELP_TEXT = "any old text will do.";

        [ExcludeFromCodeCoverage]
        [Verb("test", HelpText = VERB_HELP_TEXT)]
        public class TestCommandWithAttributes
        {
            [Option(Required = false, HelpText = OPTION_HELP_TEXT)]
            public string? Text { get; set; }
        }

        [ExcludeFromCodeCoverage]
        public class TestCommandWithoutAttributes
        {
            public string? Text { get; set; }
        }
    }
}
