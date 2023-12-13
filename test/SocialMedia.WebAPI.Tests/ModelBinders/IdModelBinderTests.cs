using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Binders;

namespace SocialMedia.WebAPI.Tests.ModelBinders
{
    public class IdModelBinderTests
    {
        private const string bindingFieldName = "id";

        private readonly IdModelBinder binder = new();
        private readonly DefaultModelBindingContext bindingContext;

        private readonly Mock<IValueProvider> valueProvider = new Mock<IValueProvider>();

        public IdModelBinderTests()
        {
            bindingContext = new DefaultModelBindingContext
            {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(
                    typeof(NumericId)
                ),
                FieldName = bindingFieldName,
                ValueProvider = valueProvider.Object,
                ModelState = new ModelStateDictionary()
            };
        }

        [Fact]
        public async Task BindModelAsync_WithIntValue_BindsModel()
        {
            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult("123"));

            await binder.BindModelAsync(bindingContext);

            var model = bindingContext.Result.Model as NumericId;
            model.Should().NotBeNull();
            model!.Value.Should().Be(123);
        }

        [Fact]
        public async Task BindModelAsync_WithStringValue_BindsModel()
        {
            bindingContext.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(
                typeof(StringId)
            );

            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult("abc"));

            await binder.BindModelAsync(bindingContext);

            var model = bindingContext.Result.Model as StringId;
            model.Should().NotBeNull();
            model!.Value.Should().Be("abc");
        }

        [Fact]
        public async Task BindModelAsync_WithGuidValue_BindsModel()
        {
            var guid = Guid.NewGuid();

            bindingContext.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(
                typeof(GuidId)
            );

            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult(guid.ToString()));

            await binder.BindModelAsync(bindingContext);

            var model = bindingContext.Result.Model as GuidId;
            model.Should().NotBeNull();
            model!.Value.Should().Be(guid);
        }

        [Fact]
        public async Task BindModelAsync_WithNullValue_FailsToBindsModel()
        {
            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(ValueProviderResult.None);

            await binder.BindModelAsync(bindingContext);

            bindingContext.Result.IsModelSet.Should().BeFalse();
        }

        [Fact]
        public async Task BindModelAsync_WithMissingField_FailsToBindsModel()
        {
            await binder.BindModelAsync(bindingContext);

            bindingContext.Result.IsModelSet.Should().BeFalse();
        }

        [Fact]
        public async Task BindModelAsync_WithEmptyStringValue_FailsToBindsModel()
        {
            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult(string.Empty));

            await binder.BindModelAsync(bindingContext);

            bindingContext.Result.IsModelSet.Should().BeFalse();
        }

        [Fact]
        public async Task BindModelAsync_WithInvalidGuidValue_FailsToBindsModel()
        {
            bindingContext.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(
                typeof(GuidId)
            );

            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult("not a guid"));

            await binder.BindModelAsync(bindingContext);

            bindingContext.Result.IsModelSet.Should().BeFalse();
        }

        [Fact]
        public async Task BindModelAsync_WithUnsupportedType_FailsToBindsModel()
        {
            bindingContext.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(
                typeof(UnsupportedId)
            );

            valueProvider
                .Setup(p => p.GetValue(bindingFieldName))
                .Returns(new ValueProviderResult("123"));

            await binder.BindModelAsync(bindingContext);

            bindingContext.Result.IsModelSet.Should().BeFalse();
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
