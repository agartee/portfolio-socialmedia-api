using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using SocialMedia.Domain.Models;
using SocialMedia.WebAPI.Binders;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SocialMedia.WebAPI.Tests.Binders
{
    public class UserIdFromClaimModelBinderTests
    {
        [Fact]
        public async Task BindModelAsync_GivenValidIdClaim_BindsModelWithIdFromClaim()
        {
            var userId = new UserId("id");
            string name = "test";
            var modelBindingContext = CreateModelBindingContext(userId, typeof(Thing),
                JsonSerializer.Serialize(new { name }));

            var binder = new UserIdFromClaimModelBinder();
            await binder.BindModelAsync(modelBindingContext);

            var model = modelBindingContext.Result.Model;
            model.Should().BeOfType<Thing>();
            model.As<Thing>().UserId.Should().Be(userId);
            model.As<Thing>().Name.Should().Be(name);
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData(null)]
        public async Task BindModelAsync_GivenNullJsonInRequestBody_BindsModelWithOnlyIdFromClaim(string? body)
        {
            var userId = new UserId("id");
            var modelBindingContext = CreateModelBindingContext(userId, typeof(Thing), body);

            var binder = new UserIdFromClaimModelBinder();
            await binder.BindModelAsync(modelBindingContext);

            var model = modelBindingContext.Result.Model;
            model.Should().BeOfType<Thing>();
            model.As<Thing>().UserId.Should().Be(userId);
            model.As<Thing>().Name.Should().BeNull();
        }

        [Fact]
        public async Task BindModelAsync_GivenInvalidRequestBody_ThrowsInvalidOperationException()
        {
            var userId = new UserId("id");
            var modelBindingContext = CreateModelBindingContext(userId, typeof(Thing), "not JSON");

            var binder = new UserIdFromClaimModelBinder();

            var action = () => binder.BindModelAsync(modelBindingContext);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        private DefaultModelBindingContext CreateModelBindingContext(UserId userId, Type modelType, string? body)
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

        private static ClaimsPrincipal CreateUser(UserId userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.Value)
            }));
        }

        private static void WriteRequestBody(string body, DefaultHttpContext httpContext)
        {
            var stream = new MemoryStream();
            var requestBytes = Encoding.UTF8.GetBytes(body);
            stream.Write(requestBytes, 0, requestBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            httpContext.Request.Body = stream;
        }

        public class Thing
        {
            public required UserId UserId { get; set; }
            public string? Name { get; set; }
        }
    }
}
