using System.Collections.Generic;
using Social_Media_Website.Models;

namespace Social_Media_Website.ViewModels
{
    public class ConversationMessagesViewModel
    {

        public int ConversationId { get; set; }
        public ICollection<Message> Messages { get; set; }

        public AppUser User1 { get; set; }
        public AppUser User2 { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public string MessageSender { get; set; }
        public string MessageReceiver { get; set; }
        public AppUser CurrentUser { get; set; }
        public string? User1Name { get; set; }
        public string? User1ProfileImageUrl { get; set; }
        public string? User2Name { get; set; }
        public string? User2ProfileImageUrl { get; set; }
      /*  public Dictionary<string, AppUser> UserDict { get; set; }*/ // map User2Id to AppUser
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageCreatedAt { get; set; }
    }
}