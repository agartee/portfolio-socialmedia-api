using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using SocialMedia.WebAPI.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;

namespace SocialMedia.WebAPI.Tests.Swagger
{
    public class RequestQueryParameterFilterTests
    {
        private static string xmlDocumentPath = Path.Combine(
            AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

        private readonly RequestQueryParameterFilter filter = new(XDocument.Load(xmlDocumentPath));
        private readonly Mock<ISchemaGenerator> schemaGeneratorMock = new();

        [Fact]
        public void Apply_WhenOperationHasRequestParameter_UpdatesParameterDescriptions()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = $"{nameof(TestRequest.Id)}.Value"
                    },
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = nameof(TestRequest.Name)
                    },
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = nameof(TestRequest.PropertyWithoutSummary)
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetByRequest));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var idParameter = operation.Parameters.First(p => p.Name == "id");
            idParameter.Description.Should().Be("ID summary");


            var nameParameter = operation.Parameters.First(p => p.Name == "name");
            nameParameter.Description.Should().Be("Name summary");

            var noSummaryParameter = operation.Parameters.First(p => p.Name == "propertyWithoutSummary");
            noSummaryParameter.Description.Should().BeNull();
        }

        [Fact]
        public void Apply_WhenOperationDoesNotHaveRequestParameter_DoesNotUpdateParameterDescriptions()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetById));
            filter.Apply(operation, CreateTestOperationContext(method!));

            operation.Parameters.First(p => p.Name == "id").Description.Should().BeNull();
        }

        private OperationFilterContext CreateTestOperationContext(MethodInfo methodInfo)
        {
            return new OperationFilterContext(
                new ApiDescription(),
                schemaGeneratorMock.Object,
                new SchemaRepository(),
                methodInfo);
        }

        private class TestController
        {
            public string GetByRequest(TestRequest request)
            {
                return request.ToString();
            }

            public string GetById(int id)
            {
                return id.ToString();
            }
        }

        private record TestRequest : IRequest<string>
        {
            /// <summary>
            /// ID summary
            /// </summary>
            public required int Id { get; init; }
            /// <summary>
            /// Name summary
            /// </summary>
            public required string Name { get; init; }

            public required string PropertyWithoutSummary { get; init; }
        }
    }
}
