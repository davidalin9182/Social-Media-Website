using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.Repository;
using Social_Media_Website.ViewModels;
using System.Security.Claims;

namespace Social_Media_Website.Controllers
{
    public class ConversationController : Controller
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public ConversationController(IConversationRepository conversationRepository, IUserRepository userRepository, UserManager<AppUser> userManager)
        {
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }
        

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var conversations = await _conversationRepository.GetConversationsForUserAsync(currentUser.Id);
            var user1Ids = conversations.Select(c => c.User1.Id).Distinct().ToList();
            var user2Ids = conversations.Select(c => c.User2Id).Distinct().ToList();

            var user1Dict = await _userManager.Users.Where(u => user1Ids.Contains(u.Id)).ToDictionaryAsync(u => u.Id);


            var user2Dict = await _userManager.Users.Where(u => user2Ids.Contains(u.Id)).ToDictionaryAsync(u => u.Id);
           
            var viewModel = new ConversationsViewModel
            {
                Conversations = conversations,

                UserDict1 = user1Dict,
                UserDict2 = user2Dict
            };

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> StartConversation(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var otherUser = await _userManager.FindByIdAsync(userId);

            if (otherUser == null)
            {
                return NotFound();
            }

            var conversation = await _conversationRepository.StartConversationAsync(currentUser.Id, userId);

            return RedirectToAction("Index", "Conversation", new { conversationId = conversation.Id });
        }

 

        [HttpPost]
        public async Task<IActionResult> SendMessage(int conversationId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);

            if (conversation == null || (!conversation.User1.Id.Equals(currentUser.Id) && !conversation.User2.Id.Equals(currentUser.Id)) || 
                (conversation.User1.Id.Equals(currentUser.Id) && conversation.User2.Id.Equals(currentUser.Id)))
            {
                return NotFound();
            }

            var message = new Message
            {
                Content = content,
                SenderId = currentUser.Id,
                SenderProfileImage = currentUser.ProfileImageUrl,
                SenderName = currentUser.UserName,
                SentAt = DateTime.UtcNow
            };

            conversation.Messages.Add(message);
            conversation.LastMessageCreatedAt = DateTime.UtcNow;
            await _conversationRepository.SaveChangesAsync();

            return Json(new { content = message.Content, senderProfileImage = message.SenderProfileImage, senderName = message.SenderName, sentAt = message.SentAt.ToShortTimeString() });


        }


    


        [HttpPost]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {
            var conversationMessagesViewModel = await _conversationRepository.GetConversationByIdAsync(conversationId);
            if (conversationMessagesViewModel == null)
            {
                return NotFound();
            }

            await _conversationRepository.DeleteConversationAsync(conversationMessagesViewModel);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConversationPartial(int conversationId)
        {
            var conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);
            var currentUser = await _userManager.GetUserAsync(User);
            if (conversation == null)
            {
                return NotFound();
            }

            var user1 = await _userRepository.GetUserById(conversation.User1.Id);
            var user2 = await _userRepository.GetUserById(conversation.User2Id);

            var viewModel = new ConversationMessagesViewModel
            {
                ConversationId = conversationId,
                Messages = conversation.Messages.OrderBy(m => m.SentAt).ToList(),
                User1 = user1,
                User2 = user2,
                User1Id = user1.Id,
                User2Id = user2.Id,
                MessageSender = (User.Identity.Name == user1.UserName) ? user1.UserName : user2.UserName,
                MessageReceiver = (User.Identity.Name == user1.UserName) ? user2.UserName : user1.UserName,
                User1Name = user1.UserName,
                User1ProfileImageUrl = user1.ProfileImageUrl ?? "/images/avatar_image_1.jpg",
                User2Name = user2.UserName,
                User2ProfileImageUrl = user2.ProfileImageUrl ?? "/images/avatar_image_1.jpg",
                CurrentUser = currentUser,
            };

            var lastMessage = conversation.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
            if (lastMessage != null)
            {
                viewModel.LastMessageContent = lastMessage.Content;
                viewModel.LastMessageCreatedAt = lastMessage.SentAt;
            }

            return PartialView("_ConversationMessages", viewModel);
        }


    }
}


