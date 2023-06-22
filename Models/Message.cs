using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_Website.Models
    {
        public class Message
        {
            public int Id { get; set; }
            public int? ConversationId { get; set; }
            public string? Content { get; set; }
            public string? ContentImage { get; set; }
            public DateTime SentAt { get; set; }
            public string SenderId { get; set; }
            public string? SenderProfileImage { get; set; }
            public string? SenderName { get; set; }
            //public string ReceiverId { get; set; }
        }
    }

