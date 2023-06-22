using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_Website.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime DateLiked { get; set; }


        public Posts Post { get; set; }
    }
}