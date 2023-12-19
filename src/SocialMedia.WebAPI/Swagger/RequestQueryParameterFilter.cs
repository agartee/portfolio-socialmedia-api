using MediatR;
using Microsoft.OpenApi.Models;
using SocialMedia.Domain.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;

namespace SocialMedia.WebAPI.Swagger
{
    public class RequestQueryParameterFilter : IOperationFilter
    {
        private const string idParameterNameSuffix = $".{nameof(Id<int>.Value)}";
        private readonly XDocument document;

        public RequestQueryParameterFilter(XDocument document)
        {
            this.document = document;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;

            var requestParameterInfo = methodInfo.GetParameters()
                .Where(p => IsRequestType(p.ParameterType))
                .SingleOrDefault();

            if (requestParameterInfo != null)
            {
                var queryParameters = operation.Parameters
                    .Where(p => p.In == ParameterLocation.Query);

                foreach (var parameter in queryParameters)
                {
                    string requestPropertyName = parameter.Name.EndsWith(idParameterNameSuffix)
                        ? parameter.Name.Substring(0, parameter.Name.IndexOf(idParameterNameSuffix))
                        : parameter.Name;

                    parameter.Name = CamelCase(RemoveDotValueSuffix(parameter.Name));

                    var xmlMemberName = CreateXmlMemberKey(requestParameterInfo, requestPropertyName);
                    if (xmlMemberName != null)
                        parameter.Description = GetXmlMemberSummaryValue(xmlMemberName);
                }
            }
        }

        private static bool IsRequestType(Type parameterType)
        {
            return typeof(IBaseRequest).IsAssignableFrom(parameterType);
        }

        private static string CreateXmlMemberKey(ParameterInfo requestParameterInfo, string propertyName)
        {
            return $"P:{requestParameterInfo.ParameterType.FullName!.Replace("+", ".")}.{propertyName}";
        }

        private string? GetXmlMemberSummaryValue(string xmlMemberName)
        {
            var parameterElement = document.Root!.Elements("members").Elements("member")
                .Where(m => m.Attribute("name")!.Value == xmlMemberName)
                .SingleOrDefault();

            return parameterElement?.Element("summary")!.Value.Trim();
        }

        private static string CamelCase(string str)
        {
            return string.Concat(str.Select((c, i) => i == 0 ? char.ToLower(c) : c));
        }

        private static string RemoveDotValueSuffix(string str)
        {
            if (str.EndsWith(idParameterNameSuffix))
                return str.Split('.')[0];

            return str;
        }
    }
}
