
namespace Social_Media_Website.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? UserName { get;set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? CoverImage { get; set; }

    }
}