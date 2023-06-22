using Social_Media_Website.Models;
using System.Web.Mvc;
using X.PagedList;

namespace Social_Media_Website.ViewModels
{
    public class CreatePostsViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PostTitle { get; set; }
        public string? PostContent { get; set; }
        public IFormFile? Image { get; set; }
        public string? AppUserId { get; set; }
        public DateTime DateCreated { get; set; }

     
    }
}
