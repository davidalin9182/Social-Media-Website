using Social_Media_Website.Models;

namespace Social_Media_Website.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? UserName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public List<Posts>? Posts { get; set; }
        public bool IsFriend { get; set; }
    }
}

