using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.TestUtilities.Exceptions;

namespace SocialMedia.Persistence.SqlServer.Tests.TestUtilities
{
    internal static class PostExtensions
    {
        internal static PostData ToPostData(this PostConfiguration post, MappingBehavior mappingBehavior = MappingBehavior.Default)
        {
            return new PostData
            {
                Id = post.Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(post.Id)),
                AuthorUserId = post.Author?.Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(post.Author.Id)),
                Created = post.Created ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(post.Created)),
                Content = new PostContentData
                {
                    PostId = post.Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(PostData.Id)),
                    Text = post.Text ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(PostData.Id)),
                },
                User = mappingBehavior.HasFlag(MappingBehavior.IncludeUser) ?
                    post.Author.ToUserData() : null
            };
        }

        [Flags]
        internal enum MappingBehavior
        {
            Default = 0,
            IncludeUser = 1
        }
    }
}
