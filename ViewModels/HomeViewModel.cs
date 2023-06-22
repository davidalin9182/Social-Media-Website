using Social_Media_Website.Models;

namespace Social_Media_Website.ViewModels
{
    public class HomeViewModel
    {

        public int Id { get; set; }
    
        public ICollection<PostsViewModel>? Posts { get; set; }
        public ICollection<CommentViewModel>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
       
        public List<AppUser>? SuggestedUsers { get; set; }

    }
}
