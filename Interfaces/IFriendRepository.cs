using Social_Media_Website.Models;

namespace Social_Media_Website.Interfaces
{
    public interface IFriendRepository
    {
        Task<List<Friend>> GetFriendsAsync(string userId);
       
        Task RemoveFriendAsync(string userId, string friendId);
       
        Task<AppUser> GetFriendUserAsync(string userId, string friendId);
        Task AddFriendAsync(string userId, string friendId, string friendUsername);
        Task<int> GetFriendsCountAsync(string userId);
        Task<List<Friend>> GetFriendsWhoFollowUserAsync(string userId);
        Task<int> GetFriendsWhoFollowUserCountAsync(string userId);

        Task<List<AppUser>> GetFriendsOfFriendsAsync(string userId);
        Task<List<AppUser>> GetMostFollowedUsersAsync(int count);
        //Task<List<Friend>> GetFriendsOfFriendsAsync(string userId);

    }
}
