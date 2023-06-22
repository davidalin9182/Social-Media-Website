using Social_Media_Website.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_Website.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string? CommentContent { get; set; }
        public DateTime DateCreated { get; set; }
       


    }
}
