﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_Website.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? Image { get; set; }


    }
}
