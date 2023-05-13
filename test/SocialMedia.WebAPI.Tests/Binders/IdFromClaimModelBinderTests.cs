using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using SocialMedia.WebAPI.Binders;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SocialMedia.WebAPI.Tests.Binders
{
    public class IdFromClaimModelBinderTests
    {
        [Fact]
        public async Task BindModelAsync_GivenValidIdClaim_BindsModelWithIdFromClaim()
        {
            var id = "id";
            string name = "test";
            var modelBindingContext = CreateModelBindingContext(id, typeof(Thing),
                JsonSerializer.Serialize(new { name }));

            var binder = new IdFromClaimModelBinder();
            await binder.BindModelAsync(modelBindingContext);

            var model = modelBindingContext.Result.Model;
            model.Should().BeOfType<Thing>();
            model.As<Thing>().Id.Should().Be(id);
            model.As<Thing>().Name.Should().Be(name);
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task BindModelAsync_GivenNullJsonInRequestBody_BindsModelWithOnlyIdFromClaim(string? body)
        {
            var id = "id";
            var modelBindingContext = CreateModelBindingContext(id, typeof(Thing), body);

            var binder = new IdFromClaimModelBinder();
            await binder.BindModelAsync(modelBindingContext);

            var model = modelBindingContext.Result.Model;
            model.Should().BeOfType<Thing>();
            model.As<Thing>().Id.Should().Be(id);
            model.As<Thing>().Name.Should().BeNull();
        }

        [Fact]
        public async Task BindModelAsync_GivenInvalidRequestBody_ThrowsInvalidOperationException()
        {
            var id = "id";
            var modelBindingContext = CreateModelBindingContext(id, typeof(Thing), "not JSON");

            var binder = new IdFromClaimModelBinder();

            var action = () => binder.BindModelAsync(modelBindingContext);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        private DefaultModelBindingContext CreateModelBindingContext(string userId, Type modelType, string? body)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = CreateUser(userId);

            if (body != null)
                WriteRequestBody(body, httpContext);

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(modelType);

            return new DefaultModelBindingContext
            {
                ActionContext = new ActionContext(httpContext, Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>()),
                ModelMetadata = modelMetadata,
                ModelState = new ModelStateDictionary(),
                ModelName = modelType.Name
            };
        }

        private static ClaimsPrincipal CreateUser(string userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
        }

        private static void WriteRequestBody(string? body, DefaultHttpContext httpContext)
        {
            var stream = new MemoryStream();
            var requestBytes = Encoding.UTF8.GetBytes(body);
            stream.Write(requestBytes, 0, requestBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            httpContext.Request.Body = stream;
        }

        public class Thing
        {
            public required string Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
