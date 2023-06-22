using Social_Media_Website.Models;
using X.PagedList;



namespace Social_Media_Website.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<PostsViewModel>? Posts { get; set; }

        public int ? FollowingCount { get; set; }
        public int? FollowersCount { get; set; }
        public List<AppUser>? SuggestedUsers { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
