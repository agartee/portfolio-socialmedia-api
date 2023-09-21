using FluentAssertions;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Extensions;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Tests.Extensions
{
    public class PostExtensionsTests
    {
        [Fact]
        public void ToPostInfo_ReturnsExpectedPostInfo()
        {
            var id = Guid.NewGuid();

            var data = new PostData
            {
                Id = id,
                Created = DateTime.Now,
                UserId = "user1",
                User = new UserData
                {
                    UserId = "user1",
                    Name = "Test User",
                    Created = DateTime.MinValue,
                    LastUpdated = DateTime.MinValue,
                },
                Content = new PostContentData
                {
                    PostId = id,
                    Text = "text"
                }
            };

            var result = data.ToPostInfo();

            result.Id.Should().Be(id);
            result.Text.Should().Be(data.Content.Text);
            result.Author.Should().Be(data.User.Name);
            result.Created.Should().Be(data.Created);
        }

        [Fact]
        public void ToPostInfo_WhenUserDataIsNotIncluded_Throws()
        {
            var id = Guid.NewGuid();

            var data = new PostData
            {
                Id = id,
                Created = DateTime.Now,
                UserId = "user1",
                Content = new PostContentData
                {
                    PostId = id,
                    Text = "text"
                }
            };

            var action = () => data.ToPostInfo();

            action.Should().Throw<ModelMappingException<PostData, PostInfo>>()
                .WithMessage($"*Missing {nameof(PostData.User)} value*");
        }
    }
}
