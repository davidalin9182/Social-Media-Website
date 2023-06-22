using Social_Media_Website.Models;
using System.Web.Mvc;
using X.PagedList;

namespace Social_Media_Website.ViewModels
{
    public class PostsViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string AppUserId { get; set; }
        public string? PostTitle { get; set; }
        [AllowHtml]
        public string? PostContent { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<PostsViewModel>? Posts { get; set; }
        public ICollection<CommentViewModel>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public int? CommentsCount { get; set; } = 0;
        public int? LikesCount { get; set; } = 0;
        public bool IsLikedByCurrentUser { get; set; } = false;
       

    }
}
