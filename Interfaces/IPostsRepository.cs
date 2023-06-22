
using Social_Media_Website.Models;
using Social_Media_Website.Data;
using Social_Media_Website.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Social_Media_Website.Interfaces
{
    public interface IPostsRepository
    {
        Task<IEnumerable<Posts>> GetAll();
        Task<IEnumerable<Posts>> GetAllPostsPaged(int pageNumber, int pageSize);
        Task<ICollection<PostsViewModel>> GetAllPosts();
        Task<Posts?> GetByIdAsync(int id);

        Task<Posts?> GetByIdAsyncNoTracking(int id);
        Task<AppUser> GetUserById(string id);
        //Task<int> GetCommentCount(int postId);
        Task<int> GetCountAsync();
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<IEnumerable<Posts>> GetPaged(int page, int pageSize);
        Task<int> Count();
        Task<List<CommentViewModel>> GetCommentsForPost(int postId);
        //Task<bool> AddCommentAsync(Comment comment);
        Task<int> GetLikesCount(int postId);
        Task<bool> UpdatePost(Posts post);
        Task<bool> SaveAsync();
        //Task<bool> AddAsync(Comment comment);
        Task<Posts> GetPostById(int postId);
       
        Task<int> GetCommentCount(int postId);
        bool Add(Posts Posts);

        bool Update(Posts Posts);

        bool Delete(Posts Posts);

        bool AddComment(Comment comment);
        bool UpdateComment(Comment comment);
        bool DeleteComment(Comment comment);

        bool Save();
        Task<bool> UpdateLikeCount(int postId, int? likeCount);
        //bool IsPostLikedByCurrentUser(int postId);

        //bool IsPostLikedByCurrentUser(int postId, string currentUserId);
        //void RemoveLike(int postId, string currentUserId);
        //void AddLike(int postId, string currentUserId);

        bool IsPostLikedByCurrentUser(Posts post, string currentUserId);
        bool RemoveLike(Posts post, string currentUserId);
        bool AddLike(Posts post, string currentUserId);

        Task<int> GetLikeCountByPostId(int postId);
        Task<List<Like>> GetLikesByPostId(int postId);
        //Task<ICollection<Like>> GetLikesByPostId(int postId);
        Task<Comment> GetCommentByIdAsync(int id);
        Task<Comment> GetCommentByIdAsyncNoTracking(int id);
        Task<ICollection<PostsViewModel>> GetFollowedPosts(string userId);
    }
}