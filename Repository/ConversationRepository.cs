using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Data;
using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting.Server;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace Social_Media_Website.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ConversationRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Conversation> StartConversationAsync(string userId1, string userId2)
        {
            // Check if conversation already exists between the two users
            var conversation = await _context.Conversation
                .Where(c => (c.User1.Id == userId1 && c.User2Id== userId2) || (c.User1.Id == userId2 && c.User2Id == userId1))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                // Conversation doesn't exist, create a new one
                var user1 = await _userManager.FindByIdAsync(userId1);
                
                conversation = new Conversation
                {
                    User1 = user1,
                    User2Id= userId2,
                    LastActivity = DateTime.Now
                };

                await _context.Conversation.AddAsync(conversation);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Conversation already exists, update last activity timestamp
                conversation.LastActivity = DateTime.Now;
                _context.Conversation.Update(conversation);
                await _context.SaveChangesAsync();
            }

            return conversation;

        }

        public async Task<List<Conversation>> GetConversationsForUserAsync(string userId)
        {
            return await _context.Conversation
                .Include(c => c.User1)
                .Include(c => c.Messages)

                .Where(c => c.User1.Id == userId || c.User2Id == userId)
                .OrderByDescending(c => c.LastActivity)
                .ToListAsync();
        }

        public async Task<ConversationMessagesViewModel> GetConversationByIdAsync(int id)
        {
            var conversation = await _context.Conversation
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .FirstOrDefaultAsync(c => c.Id == id);

            var user2 = await _userManager.FindByIdAsync(conversation.User2Id);

            var conversationMessagesViewModel = new ConversationMessagesViewModel
            {
                ConversationId = conversation.Id,
                Messages = conversation.Messages,
                User1 = conversation.User1,
                User2 = user2,
                User1Id = conversation.User1.Id,
                User2Id = conversation.User2Id,
                User1Name = conversation.User1.UserName,
                User1ProfileImageUrl = conversation.User1.ProfileImageUrl,
                User2Name = user2.UserName,
                User2ProfileImageUrl = user2.ProfileImageUrl,
                //UserDict = new Dictionary<string, AppUser>
                //{
                //    { conversation.User1.Id, conversation.User1 },
                //    { conversation.User2Id, user2 }
                //}
            };

            return conversationMessagesViewModel;
        }

        //public Conversation GetConversationWithMessages(int conversationId)
        //{
        //    return _context.Conversation
        //                   .Include(c => c.Messages)
        //                    .Include(c => c.User1)
        //                    .Include(c => c.User2Id)
        //                   .FirstOrDefault(c => c.Id == conversationId);
        //}

        public Conversation GetConversationWithMessages(int conversationId)
        {
            return _context.Conversation
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == conversationId);
        }

        public async Task DeleteConversationAsync(ConversationMessagesViewModel conversationMessagesViewModel)
        {
            var conversation = await _context.Conversation.FindAsync(conversationMessagesViewModel.ConversationId);

            if (conversation != null)
            {
                _context.Conversation.Remove(conversation);
                await _context.SaveChangesAsync();
            }
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
