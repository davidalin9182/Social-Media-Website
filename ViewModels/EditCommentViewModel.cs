using Social_Media_Website.Models;
using System.Web.Mvc;

namespace Social_Media_Website.ViewModels
{
    public class EditCommentViewModel
    {

        public int Id { get; set; }
        public int PostId { get; set; }
        public string? UserName { get; set; }
        public AppUser AppUser { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CommentContent { get; set; }
        public string? AppUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
