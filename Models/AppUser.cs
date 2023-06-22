using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Social_Media_Website.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Social_Media_Website.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public ICollection<PostsViewModel>? Posts { get; set; }
        public ICollection<Conversation>? Conversations { get; set; }
        public ICollection<CommentViewModel>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }

        public DateTime CreationDate { get; set; }

    }

  

}

