using FluentAssertions;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Extensions;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Persistence.SqlServer.Tests.Extensions
{
    public class PostExtensionsTests
    {
        private readonly PostBuilder postBuilder = new();

        [Fact, UseMappingContextScope]
        public void ToPostInfo_ReturnsExpectedPostInfo()
        {
            var post = postBuilder.CreatePost();
            var postData = post.ToPostData();

            var result = postData.ToPostInfo();

            result.Should().Be(post.ToPostInfo());
        }

        [Fact]
        public void ToPostInfo_WhenUserDataIsNotIncluded_Throws()
        {
            var id = Guid.NewGuid();

            var data = new PostData
            {
                Id = id,
                Created = DateTime.Now,
                AuthorUserId = "user1",
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
