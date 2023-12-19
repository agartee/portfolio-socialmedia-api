using Microsoft.OpenApi.Models;
using SocialMedia.Domain.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;

namespace SocialMedia.WebAPI.Swagger
{
    public class IdPathParameterFilter : IOperationFilter
    {
        private readonly XDocument document;

        public IdPathParameterFilter(XDocument document)
        {
            this.document = document;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.In == ParameterLocation.Path)
                {
                    var idParameterInfo = methodInfo.GetParameters()
                        .Where(p => p.Name == parameter.Name)
                        .Where(p => IsIdType(p.ParameterType))
                        .SingleOrDefault();

                    if (idParameterInfo != null)
                    {
                        var xmlMemberName = CreateXmlMemberKey(methodInfo);
                        parameter.Description = GetXmlMemberSummaryValue(xmlMemberName, parameter.Name);
                    }
                }
            }
        }

        private bool IsIdType(Type contextType)
        {
            return Id.SupportedTypes.Any(
                t => typeof(Id<>).MakeGenericType(t).IsAssignableFrom(contextType.GetTypeInfo())
            );
        }

        private static string CreateXmlMemberKey(MethodInfo methodInfo)
        {
            var className = methodInfo.DeclaringType!.FullName!.Replace("+", ".");

            var methodName = methodInfo.IsGenericMethod
                ? $"{methodInfo.Name}``{methodInfo.GetGenericArguments().Length}"
                : methodInfo.Name;

            var parameterTypeNames = string.Join(",", methodInfo.GetParameters()
                .Select(p => p.ParameterType.FullName!.Replace("&", "@").Replace("+", ".")));

            return $"M:{className}.{methodName}({parameterTypeNames})";
        }

        private string? GetXmlMemberSummaryValue(string xmlMemberName, string parameterName)
        {
            var parameterElement = document.Root!.Elements("members").Elements("member")
                .Where(m => m.Attribute("name")!.Value == xmlMemberName)
                .SingleOrDefault();

            return (parameterElement?.Elements("param")
                .SingleOrDefault(p => p.Attribute("name")!.Value == parameterName))?
                .Value.Trim();
        }
    }
}
