using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;

namespace Social_Media_Website.Interfaces
{
    public interface IConversationRepository
    {

        Task<Conversation> StartConversationAsync(string userId1, string userId2);

        Task<List<Conversation>> GetConversationsForUserAsync(string userId);
        Task<ConversationMessagesViewModel> GetConversationByIdAsync(int id);
        Conversation GetConversationWithMessages(int conversationId);
        Task DeleteConversationAsync(ConversationMessagesViewModel conversationMessagesViewModel);
        Task SaveChangesAsync();
    }
}
