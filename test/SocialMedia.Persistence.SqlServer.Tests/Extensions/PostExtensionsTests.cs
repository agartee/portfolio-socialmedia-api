using FluentAssertions;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Extensions;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Builders;
using static SocialMedia.TestUtilities.Builders.PostConfiguration;

namespace SocialMedia.Persistence.SqlServer.Tests.Extensions
{
    public class PostExtensionsTests
    {
        private readonly PostBuilder postBuilder = new();

        [Fact]
        public void ToPostInfo_ReturnsExpectedPostInfo()
        {
            var post = postBuilder.CreatePost();
            var postData = post.ToPostData(MappingBehavior.IncludeUser);

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

    public class UserExtensionsTests
    {
        private readonly UserBuilder userBuilder = new();

        [Fact]
        public void ToUser_ReturnsExpectedUserInfo()
        {
            var user = userBuilder.CreateUser();
            var userData = user.ToUserData();

            var result = userData.ToUser();

            result.Should().Be(user.ToUser());
        }
    }
}
