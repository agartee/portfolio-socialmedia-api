using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Persistence.SqlServer.Models
{
    [Table(TABLE_NAME)]
    public class PostContentData
    {
        public const string TABLE_NAME = "PostContent";

        [Key]
        public required Guid PostId { get; set; }
        public required string Text { get; set; }

        public PostData? Post { get; set; }
    }
}
