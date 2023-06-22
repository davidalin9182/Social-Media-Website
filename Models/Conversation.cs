using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_Website.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastActivity { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public AppUser User1 { get; set; }
        public string? User2Id { get; set; }
    }
}
