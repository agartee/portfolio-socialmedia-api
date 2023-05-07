using CommandLine;
using MediatR;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace SocialMedia.Domain.Services
{
    public interface ICliRequestBuilder
    {
        IBaseRequest BuildRequest(string commandText);
    }

    public class CliRequestBuilder : ICliRequestBuilder
    {
        private readonly Type[] requestTypes;

        public CliRequestBuilder(params Type[] requestTypes)
        {
            this.requestTypes = requestTypes;
        }

        public IBaseRequest BuildRequest(string commandText)
        {
            var commandArgs = SplitCommandArgs(commandText);

            if (commandText == "--help")
                return new GetHelpText();

            try
            {
                var parserResult = Parser.Default.ParseArguments(
                    commandArgs,
                    requestTypes);

                if (parserResult.Errors.Any(e => e is HelpRequestedError))
                    return new GetHelpText(parserResult);

                if (parserResult is not Parsed<object> parser)
                    throw new CommandLineParsingException(parserResult);

                if (parser.Value is not IBaseRequest)
                    throw new InvalidRequestTypeException(parser.Value.GetType());

                return (IBaseRequest)parser.Value;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new CommandLineParsingException(ex);
            }
        }

        private static string[] SplitCommandArgs(string str)
        {
            return new Regex(@"^((?!\s\-\-).)*$|^.+?(?=\s\-\-)|([^\s""]+|""[^""\\]*(?:\\.[^""\\]*)*"")")
                .Matches(str).OfType<Match>()
                .Select(m => m.Value.TrimSingle('"'))
                .Select(s => s.Replace("\\\"", "\""))
                .ToArray();
        }
    }

    public static class StringExtensions
    {
        public static string TrimSingle(this string str, char trimChar)
        {
            if (str.StartsWith(trimChar))
                str = str[1..];

            if (str.EndsWith(trimChar))
                str = str[..^1];

            return str;
        }
    }
}
