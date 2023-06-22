using System.ComponentModel.DataAnnotations;

namespace Social_Media_Website.Models
{
    public class Friend
    {
        [Key]
        public int FriendshipId { get; set; }

        public string UserId { get; set; }
        public string FriendId { get; set; }
        public string FriendUsername { get; set; }
        public AppUser Appuser { get;  set; }
    }
}
