using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;

namespace SocialMedia.WebAPI.Tests.Swagger
{
    public class IdPathParameterFilterTests
    {
        private static string xmlDocumentPath = Path.Combine(
            AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

        private readonly IdPathParameterFilter filter = new(XDocument.Load(xmlDocumentPath));
        private readonly Mock<ISchemaGenerator> schemaGeneratorMock = new();

        [Fact]
        public void Apply_WhenOperationParameterFromPathAndIsId_UpdatesParameterDescription()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Path,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetById));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var parameter = operation.Parameters.First();
            parameter.Description.Should().Be("id parameter summary");
        }

        [Fact]
        public void Apply_WhenOperationMethodIsGeneric_UpdatesParameterDescription()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Path,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetByIdGeneric));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var parameter = operation.Parameters.First();
            parameter.Description.Should().Be("id parameter summary");
        }

        [Fact]
        public void Apply_WhenOperationParameterHasNoSummary_DoesNotUpdateParameterDescription()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Path,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetByIdWithNoSummary));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var parameter = operation.Parameters.First();
            parameter.Description.Should().BeNull();
        }

        [Fact]
        public void Apply_WhenOperationParameterHasNoSummaryAndContainingClassHasNoDocumentedMembers_DoesNotUpdateParameterDescription()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Path,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestControllerWithNoSummaries.GetByIdWithNoSummary));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var parameter = operation.Parameters.First();
            parameter.Description.Should().BeNull();
        }

        [Fact]
        public void Apply_WhenOperationParameterNotFromPath_DoesNotUpdateParameterDescription()
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

            var parameter = operation.Parameters.First();
            parameter.Description.Should().BeNull();
        }

        [Fact]
        public void Apply_WhenOperationParameterNotId_DoesNotUpdateParameterDescription()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Path,
                        Name = "id"
                    }
                }
            };

            var method = typeof(TestController).GetMethod(nameof(TestController.GetByName));
            filter.Apply(operation, CreateTestOperationContext(method!));

            var parameter = operation.Parameters.First();
            parameter.Description.Should().BeNull();
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
            /// <summary>
            /// Method summary
            /// </summary>
            /// <param name="id">id parameter summary</param>
            /// <returns></returns>
            public string GetById(TestId id)
            {
                return id.ToString();
            }

            /// <summary>
            /// Method summary
            /// </summary>
            /// <param name="name">name parameter summary</param>
            /// <returns></returns>
            public string GetByName(string name)
            {
                return name;
            }

            public string GetByIdWithNoSummary(TestId id)
            {
                return id.ToString();
            }

            /// <summary>
            /// Method summary
            /// </summary>
            /// <param name="id">id parameter summary</param>
            /// <returns></returns>
            public string GetByIdGeneric<T>(TestId id) where T : class
            {
                return id.ToString();
            }
        }

        private class TestControllerWithNoSummaries
        {
            public string GetByIdWithNoSummary(TestId id)
            {
                return id.ToString();
            }
        }

        private record TestId : Id<int>
        {
            public TestId(int value) : base(value) { }
        }
    }
}
