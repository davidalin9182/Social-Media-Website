
using System.Web.Mvc;

namespace Social_Media_Website.ViewModels
{
    public class EditPostsViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PostTitle { get; set; }
        public string? PostContent { get; set; }
        public IFormFile? Image { get; set; }
        public string? URL { get; set; }
       
        public string? AppUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

   