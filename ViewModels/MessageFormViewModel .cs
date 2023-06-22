using System.ComponentModel.DataAnnotations;

namespace Social_Media_Website.ViewModels
{
    public class MessageFormViewModel
    {
        [Required(ErrorMessage = "Please enter a message")]
        public string Message { get; set; }

        [Display(Name = "Upload Image")]
        public string Image { get; set; }

        public string Content { get; set; }
        public string RecipientId { get; set; }
    }
}
