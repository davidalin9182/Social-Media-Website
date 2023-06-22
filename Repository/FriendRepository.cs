using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Data;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;

namespace Social_Media_Website.Repository
{
    public class FriendRepository: IFriendRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FriendRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Friend>> GetFriendsAsync(string userId)
        {
            return await _context.Friend
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetFriendsCountAsync(string userId)
        {
            return await _context.Friend
                .CountAsync(f => f.UserId == userId);
        }
        public async Task<List<Friend>> GetFriendsWhoFollowUserAsync(string userId)
        {
            return await _context.Friend
                .Where(f => f.FriendId == userId)
                .ToListAsync();
        }

        public async Task<int> GetFriendsWhoFollowUserCountAsync(string userId)
        {
            return await _context.Friend
                .Where(f => f.FriendId == userId)
                .CountAsync();
        }

        

        public async Task<List<AppUser>> GetFriendsOfFriendsAsync(string userId)
        {
            var friends = await _context.Friend.Where(f => f.UserId == userId).ToListAsync();
            var friendIds = friends.Select(f => f.FriendId).ToList();

            var friendsOfFriendsIds = await _context.Friend
                .Where(f => friendIds.Contains(f.UserId))
                .Select(f => f.FriendId)
                .ToListAsync();

            // Exclude friends that the current user is already following
            var suggestedFriendIds = friendsOfFriendsIds.Except(friendIds).ToList();

            // Exclude the current user
            suggestedFriendIds.Remove(userId);

            var suggestedUsers = await _context.Users
                .Where(u => suggestedFriendIds.Contains(u.Id))
                .Take(20)
                .ToListAsync();

            return suggestedUsers;
        }




        public async Task AddFriendAsync(string userId, string friendId , string friendUsername)
        {
            if (_context == null)
            {
                throw new NullReferenceException("_context is null");
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (friendId == null)
            {
                throw new ArgumentNullException(nameof(friendId));
            }

            if (friendUsername == null)
            {
                throw new ArgumentNullException(nameof(friendUsername));
            }
            var friend = new Friend { UserId = userId, FriendId = friendId ,FriendUsername=friendUsername};
            _context.Friend.Add(friend);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendAsync(string userId, string friendId)
        {
            var friend = await _context.Friend
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

            if (friend != null)
            {
                _context.Friend.Remove(friend);
                await _context.SaveChangesAsync();
            }
        }

     
        public async Task<AppUser> GetFriendUserAsync(string userId, string friendId)
        {
            var friend = await _context.Friend
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

            if (friend != null)
            {
                return await _context.Users.FindAsync(friendId);
            }

            return null;
        }

        public async Task<List<AppUser>> GetMostFollowedUsersAsync(int count)
        {
            var mostFollowedUserIds = await _context.Friend
                .GroupBy(f => f.FriendId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToListAsync();

            var mostFollowedUsers = await _context.Users
                .Where(u => mostFollowedUserIds.Contains(u.Id))
                .ToListAsync();

            return mostFollowedUsers;
        }



    }
}
