using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Extensions
{
    public static class PostExtensions
    {
        public static PostInfo ToPostInfo(this PostData post)
        {
            return new PostInfo
            {
                Id = new PostId(post.Id),
                Author = post.User?.Name ?? throw new ModelMappingException<PostData, PostInfo>($"Missing {nameof(PostData.User)} value."),
                Created = post.Created,
                Text = post.Content.Text
            };
        }

        public static PostData ToPostData(this Post post)
        {
            return new PostData
            {
                Id = post.Id.Value,
                AuthorUserId = post.AuthorUserId.Value,
                Created = post.Created,
                Content = new PostContentData
                {
                    PostId = post.Id.Value,
                    Text = post.Text
                }
            };
        }
    }
}
