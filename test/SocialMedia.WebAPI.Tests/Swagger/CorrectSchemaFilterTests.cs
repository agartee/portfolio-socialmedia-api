using FluentAssertions;
using Microsoft.OpenApi.Models;
using Moq;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SocialMedia.WebAPI.Tests.Swagger
{
    public class CorrectSchemaFilterTests
    {
        private readonly CorrectSchemaFilter filter;
        private readonly Mock<ISchemaGenerator> schemaGeneratorMock = new();


        public CorrectSchemaFilterTests()
        {
            filter = new CorrectSchemaFilter();
        }

        [Theory]
        [InlineData(typeof(TestId), "number")]
        [InlineData(typeof(GenericTestId<int>), "number")]
        [InlineData(typeof(GenericTestId<string>), "string")]
        [InlineData(typeof(GenericTestId<Guid>), "string")]
        public void Apply_WhenContextTypeIsId_SetsSchemaTypeAndClearsProperties(Type idType, string expectedSchemaType)
        {
            var openApiSchema = new OpenApiSchema();
            var schemaFilterContext = new SchemaFilterContext(
                idType,
                schemaGeneratorMock.Object,
                new SchemaRepository());

            filter.Apply(openApiSchema, schemaFilterContext);

            openApiSchema.Type.Should().Be(expectedSchemaType);
            openApiSchema.Properties.Should().BeEmpty();
        }

        private record GenericTestId<T> : Id<T> where T : notnull
        {
            public GenericTestId(T id) : base(id) { }
        }

        private record TestId : Id<int>
        {
            public TestId(int id) : base(id) { }
        }
    }
}
