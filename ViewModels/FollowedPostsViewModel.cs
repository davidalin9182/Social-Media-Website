namespace Social_Media_Website.ViewModels
{
    public class FollowedPostsViewModel
    {
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public ICollection<PostsViewModel>? Posts { get; set; }
    }
}
