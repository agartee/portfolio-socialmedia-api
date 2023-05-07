using CommandLine;
using CommandLine.Text;
using MediatR;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Commands
{
    public class GetHelpText : IRequest<string>
    {
        public GetHelpText(ParserResult<object>? parserResult = null)
        {
            ParserResult = parserResult;
        }

        public ParserResult<object>? ParserResult { get; }
    }

    public class GetHelpTextHandler : IRequestHandler<GetHelpText, string>
    {
        private readonly HelpTextConfiguration config;

        public GetHelpTextHandler(HelpTextConfiguration config)
        {
            this.config = config;
        }

        public Task<string> Handle(GetHelpText request, CancellationToken cancellationToken)
        {
            string result = request.ParserResult != null
                ? GetRequestHelpText(request.ParserResult)
                : GetGeneralHelpText();

            return Task.FromResult(result);
        }

        private string GetRequestHelpText(ParserResult<object> parserResult)
        {
            var verbAttr = Attribute.GetCustomAttribute(parserResult.TypeInfo.Current,
                            typeof(VerbAttribute)) as VerbAttribute;

            return HelpText.AutoBuild(parserResult, h =>
            {
                if (verbAttr != null)
                    h.AddPreOptionsText(verbAttr.HelpText);

                h.Heading = config.Heading;
                h.Copyright = config.Copyright + Environment.NewLine;
                h.AdditionalNewLineAfterOption = false;
                h.AutoVersion = false;
                return h;
            }, e => e).ToString();
        }

        private string GetGeneralHelpText()
        {
            var helpText = new HelpText(
                SentenceBuilder.Create(),
                config.Heading,
                config.Copyright);

            helpText.AutoVersion = false;
            helpText.AutoHelp = false;

            if (config.Types.Any())
            {
                var sortedTypes = config.Types.Select(t =>
                {
                    var verbAttr = Attribute.GetCustomAttribute(t,
                        typeof(VerbAttribute)) as VerbAttribute;

                    return new { Type = t, Verb = verbAttr!.Name };
                }).OrderBy(x => x.Verb)
                .Select(x => x.Type)
                .ToArray();

                helpText.AddVerbs(sortedTypes);

                helpText.AddPostOptionsLine(
                    "\"[command] --help\" will display additional help information about specific commands.");
            }

            return helpText.ToString();
        }
    }
}
