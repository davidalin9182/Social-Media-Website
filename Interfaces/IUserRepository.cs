using Social_Media_Website.Models;

namespace Social_Media_Website.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<List<AppUser>> GetAllUsers2();
        Task<List<Posts>> GetUserPosts(string userId);
        Task<AppUser> GetUserById(string id);
        Task<IEnumerable<Posts>> GetAll();
        Task<IEnumerable<AppUser>> GetAllUsersByLocation(string country, string city);
        Task<IEnumerable<AppUser>> GetAllUsersExcept(IEnumerable<string> friendIds);
        Task<AppUser> GetUserAsync(string username);
        Task<IEnumerable<AppUser>> GetByIdsAsync(IEnumerable<string> ids);
        Task<IEnumerable<AppUser>> GetUserByName(string userName);
        Task<List<AppUser>> GetUserByName2(string userName);
         bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
    }
}
