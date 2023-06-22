using Social_Media_Website.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Social_Media_Website.Models
{
    public class Posts
    {
        [Key]
        public int Id { get; set; }
        // maybe name them something else
        public string? PostName { get; set; }
        public string? PostProfileImage { get; set; }
        public string? PostTitle { get; set; }
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? PostContent { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; }
     
        public ICollection<CommentViewModel>? Comments { get; set; }
        [InverseProperty("Post")]
        public ICollection<Like>? Likes { get; set; }
        public int? CommentsCount { get; set; } = 0;
        public int? LikesCount { get; set; } = 0;


    }
}


