using Social_Media_Website.Models;

namespace Social_Media_Website.ViewModels
{
    public class ConversationsViewModel
    {
        public int Id { get; set; }
        public List<Conversation>? Conversations { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastActivity { get; set; }
        //public int SelectedConversationId { get; set; }
        //public AppUser User1 { get; set; }
        //public AppUser User2 { get; set; }
        //public string User1Id { get; set; }
        //public string User2Id { get; set; }
        public ConversationMessagesViewModel conversationMessagesViewModel { get; set; }
        //public string? User1Name { get; set; }
        //public string? User1ProfileImageUrl { get; set; }
        public Dictionary<string, AppUser> UserDict1 { get; set; }
        public Dictionary<string, AppUser> UserDict2 { get; set; } // map User2Id to AppUser
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageCreatedAt { get; set; }
    }
}

