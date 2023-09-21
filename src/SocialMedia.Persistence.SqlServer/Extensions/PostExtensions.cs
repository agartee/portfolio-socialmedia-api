using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Extensions
{
    public static class PostExtensions
    {
        public static PostInfo ToPostInfo(this PostData post)
        {
            if (post.User == null)
                throw new ModelMappingException<PostData, PostInfo>($"Missing {nameof(PostData.User)} value.");

            return new PostInfo
            {
                Id = post.Id,
                Author = post.User!.Name,
                Created = post.Created,
                Text = post.Content.Text
            };
        }
    }
}
