using FluentAssertions;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.JsonConverters;
using System.Text.Json;

namespace SocialMedia.WebAPI.Tests.JsonConverters
{
    public class IdConverterTests
    {
        private readonly IdJsonConverter idConverter = new();
        private readonly JsonSerializerOptions options = new();

        public IdConverterTests()
        {
            options.Converters.Add(idConverter);
        }

        [Fact]
        public void CanConvert_WhenTypeToConvertIsId_ReturnsTrue()
        {
            var typeToConvert = typeof(PrimitiveId);

            var result = idConverter.CanConvert(typeToConvert);

            result.Should().BeTrue();
        }

        [Fact]
        public void CanConvert_WhenTypeToConvertIsNotId_ReturnsFalse()
        {
            var typeToConvert = typeof(int);

            var result = idConverter.CanConvert(typeToConvert);

            result.Should().BeFalse();
        }

        [Fact]
        public void Read_WithPrimitiveId_ReturnsHydratedIdModel()
        {
            var json = "123";

            var result = JsonSerializer.Deserialize<PrimitiveId>(json, options);

            result.Should().Be(new PrimitiveId(123));
        }

        [Fact]
        public void Read_WithGuidId_ReturnsHydratedIdModel()
        {
            var guid = Guid.NewGuid();
            var json = $"\"{guid}\"";

            var result = JsonSerializer.Deserialize<GuidId>(json, options);

            result.Should().Be(new GuidId(guid));
        }

        [Fact]
        public void Read_WithInvalidId_Throws()
        {
            var json = "123";

            var action = () => JsonSerializer.Deserialize<InvalidId>(json, options);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{nameof(InvalidId)}*");
        }

        [Fact]
        public void Write_WithPrimitiveId_ReturnsFlattenedJson()
        {
            var id = new PrimitiveId(123);

            var result = JsonSerializer.Serialize(id, options);

            result.Should().Be("123");
        }

        [Fact]
        public void Write_WithGuidId_ReturnsFlattenedJson()
        {
            var guid = Guid.NewGuid();
            var id = new GuidId(guid);

            var result = JsonSerializer.Serialize(id, options);

            result.Should().Be($"\"{guid}\"");
        }

        private record PrimitiveId : Id<int>
        {
            public PrimitiveId(int value) : base(value) { }
        }

        private record GuidId : Id<Guid>
        {
            public GuidId(Guid value) : base(value) { }
        }

        private record InvalidId : Id<int>
        {
            // why invalid? no single-parameter constructor
            public InvalidId() : base(100) { }
        }
    }
}
