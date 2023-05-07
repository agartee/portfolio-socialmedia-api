using CommandLine;
using CommandLine.Text;

namespace SocialMedia.Domain.Exceptions
{
    public class CommandLineParsingException : Exception
    {
        private const string MESSAGE = "Unable to execute command. {0}";

        public CommandLineParsingException(ParserResult<object> parserResult)
            : base(string.Format(MESSAGE, GetErrorDetails(parserResult)))
        {
        }

        public CommandLineParsingException(Exception innerException)
            : base(string.Format(MESSAGE, innerException))
        {
        }

        private static string GetErrorDetails(ParserResult<object> parserResult)
        {
            var builder = SentenceBuilder.Create();
            var errorMessages = HelpText.RenderParsingErrorsTextAsLines(
                parserResult, builder.FormatError, builder.FormatMutuallyExclusiveSetErrors, 1);

            return string.Join(" ", errorMessages).Trim();
        }
    }
}
