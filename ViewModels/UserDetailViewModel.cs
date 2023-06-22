using Social_Media_Website.Models;

namespace Social_Media_Website.ViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public List<PostsViewModel>? Posts { get; set; }
        public bool IsFriend { get; set; }
        public int? FollowingCount { get; set; }
        public int? FollowersCount { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<CommentViewModel>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public int? CommentsCount { get; set; } = 0;
        public int? LikesCount { get; set; } = 0;
        public bool IsLikedByCurrentUser { get; set; } = false;
        public List<AppUser>? SuggestedUsers { get; set; }
    }
}

