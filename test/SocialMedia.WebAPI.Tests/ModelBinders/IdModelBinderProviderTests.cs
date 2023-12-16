using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.ModelBinders;

namespace SocialMedia.WebAPI.Tests.ModelBinders
{
    public class IdModelBinderProviderTests
    {
        private readonly IdModelBinderProvider provider = new();

        [Theory]
        [InlineData(typeof(NumericId))]
        [InlineData(typeof(StringId))]
        [InlineData(typeof(GuidId))]
        public void GetBinder_WhenContextTypeIsSupported_ReturnsProvider(Type type)
        {
            var result = provider.GetBinder(CreateModelBinderProviderContext(type));

            result.Should().BeOfType<IdModelBinder>();
        }

        [Fact]
        public void GetBinder_WhenContextTypeIsNotSupported_ReturnsProvider()
        {
            var result = provider.GetBinder(
                CreateModelBinderProviderContext(typeof(UnsupportedId))
            );

            result.Should().BeNull();
        }

        [Fact]
        public void GetBinder_WhenContextIsNull_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var action = () => provider.GetBinder(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        private static ModelBinderProviderContext CreateModelBinderProviderContext(Type modelType)
        {
            var context = new Mock<ModelBinderProviderContext>();
            context
                .Setup(c => c.Metadata)
                .Returns(new EmptyModelMetadataProvider().GetMetadataForType(modelType));

            return context.Object;
        }

        private record NumericId : Id<int>
        {
            public NumericId(int value) : base(value) { }
        }

        private record StringId : Id<string>
        {
            public StringId(string value) : base(value) { }
        }

        private record GuidId : Id<Guid>
        {
            public GuidId(Guid value) : base(value) { }
        }

        private class UnsupportedId
        {
            // no constructor (reason for non-support)
            public int Value { get; init; }
        }
    }
}
