using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;
using X.PagedList;
namespace Social_Media_Website.Interfaces
{
    public interface IProfileRepository
    {
        //Task<List<Posts>> GetAllUserPosts();
        Task<AppUser> GetUserById(string id);
        //public AppUser GetUserById(string id);
        Task<AppUser> GetByIdNoTracking(string id);
        //Task<IPagedList<Posts>> GetAllUserPosts(int pageNumber, int pageSize);
        Task<ICollection<PostsViewModel>> GetAllUserPosts();
        //Task<IPagedList<PostsViewModel>> GetAllUserPosts(int pageNumber, int pageSize);
        bool Update(AppUser user);
        bool Save();


    }
}
