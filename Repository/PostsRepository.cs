using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Data;
using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;
using X.PagedList;
using Microsoft.Extensions.Hosting;

namespace Social_Media_Website.Repository
{
    public class PostsRepository : IPostsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFriendRepository _friendRepository;

        public PostsRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IFriendRepository friendRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _friendRepository = friendRepository;
        }

        public bool Add(Posts Posts)
        {
            _context.Add(Posts);
            return Save();
        }
        public bool Update(Posts Posts)
        {
            _context.Update(Posts);
            return Save();
        }

        public bool Delete(Posts Posts)
        {
            _context.Remove(Posts);
            return Save();
        }
        public async Task<bool> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Handle any exceptions that occurred during saving
                return false;
            }
        }

     
        public bool AddComment(Comment comment)
        {
            try
            {
                _context.Add(comment);
                return Save();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"An error occurred while adding a comment: {ex.Message}");
                return false;
            }
        }


        public bool UpdateComment(Comment comment)
        {
            _context.Update(comment);
            return Save();
        }

        public bool DeleteComment(Comment comment)
        {
            _context.Remove(comment);
            return Save();
        }

        public async Task<bool> UpdatePost(Posts post)
        {
            try
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exception that occurred during the save operation
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<IEnumerable<Posts>> GetAll()
        {
            return await _context.Posts.OrderByDescending(p => p.DateCreated).ToListAsync();
        }
        public async Task<IEnumerable<Posts>> GetAllPostsPaged(int pageNumber, int pageSize)
        {
            var posts = await _context.Posts.OrderByDescending(p => p.DateCreated).ToListAsync();

            return await posts.ToPagedListAsync(pageNumber, pageSize);
        }
        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public async Task<int> GetLikesCount(int postId)
        {
            // Retrieve the likes count for the specified post
            var likesCount = await _context.Likes
                .Where(l => l.PostId == postId)
                .CountAsync();

            return likesCount;
        }


        public async Task<int> GetCommentCount(int postId)
        {
            try
            {
                // Retrieve the comments for the specified post
                var comments = await _context.Comments
                    .Where(c => c.PostId == postId)
                    .ToListAsync();

                // Return the count of the comments
                return comments.Count;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                return 0;
            }
        }
        public async Task<int> GetLikeCountByPostId(int postId)
        {
            try
            {
                // Retrieve the comments for the specified post
                var likes = await _context.Likes
                    .Where(c => c.PostId == postId)
                    .ToListAsync();

                // Return the count of the comments
                return likes.Count;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                return 0;
            }
        }
        public async Task<List<Like>> GetLikesByPostId(int postId)
        {
            // Retrieve the post from the database
            var post = await _context.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                return post.Likes.ToList();
            }

            return new List<Like>(); // Return an empty list if the post is not found or has no likes
        }


        public async Task<int> GetCountAsync()
        {
            return await _context.Posts.CountAsync();
        }

        public async Task<Posts> GetByIdAsync(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Posts> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> GetCommentByIdAsyncNoTracking(int id)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<IEnumerable<Posts>> GetPaged(int page, int pageSize)
        {
            return await _context.Posts
                .OrderByDescending(p => p.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
 
        public async Task<Posts> GetPostById(int postId)
        {
            return await _context.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }



        public async Task<int> Count()
        {
            return await _context.Posts.CountAsync();
        }


        public async Task<List<CommentViewModel>> GetCommentsForPost(int postId)
        {
            
            var comments = await _context.Comments
                .Include(c => c.AppUser) // Include the related AppUser entity
                .Where(c => c.PostId == postId)
                .ToListAsync();

          
           

            var commentViewModels = comments.Select(comment => new CommentViewModel
            {

                Id = comment.Id,
                PostId = comment.PostId,
                AppUser = comment.AppUser,
                CommentContent = comment.CommentContent,
                DateCreated = comment.DateCreated,
                ProfileImageUrl = comment.AppUser.ProfileImageUrl, 
                UserName = comment.AppUser.UserName 
            }).ToList();

            return commentViewModels;
        }

        public async Task<bool> UpdateLikeCount(int postId, int? likeCount)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post != null)
            {
                post.LikesCount = likeCount;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }



        public bool IsPostLikedByCurrentUser(Posts post, string currentUserId)
        {
            if (post != null && post.Likes != null && post.Likes.Any())
            {
                // Check if the current user has liked the post
                var like = post.Likes.FirstOrDefault(l => l.UserId == currentUserId);
                return like != null;
            }

            return false;
        }



        public bool RemoveLike(Posts post, string currentUserId)
        {
            if (post != null)
            {
                // Find the like by the current user and remove it
                var like = post.Likes.FirstOrDefault(l => l.UserId == currentUserId);
                if (like != null)
                {
                    post.Likes.Remove(like);
                    _context.SaveChanges(); // Save changes to persist the removal
                    return true;
                }
            }
            return false;
        }


        public bool AddLike(Posts post, string currentUserId)
        {
            if (post != null)
            {
                // Initialize the Likes collection if null
                if (post.Likes == null)
                {
                    post.Likes = new List<Like>();
                }

                // Add a new like for the current user
                var like = new Like { PostId = post.Id, UserId = currentUserId };
                post.Likes.Add(like);
                return Save();
            }
            return false;
        }

        public async Task<ICollection<PostsViewModel>> GetAllPosts()
        {
            var userPosts = _context.Posts
                .OrderByDescending(p => p.DateCreated)
                .Select(p => new PostsViewModel
                {
                    Id = p.Id,
                    PostTitle = p.PostTitle,
                    PostContent = p.PostContent,
                    AppUserId = p.AppUserId,
                    Image = p.Image,
                    CommentsCount = p.CommentsCount,
                    LikesCount = p.LikesCount,
                    Likes = p.Likes
                });

            return await userPosts.ToListAsync();
        }



        public async Task<ICollection<PostsViewModel>> GetFollowedPosts(string userId)
        {
            var friendList = await _friendRepository.GetFriendsAsync(userId);
            var friendIds = friendList.Select(friend => friend.FriendId);

            var followedPosts = _context.Posts
                .Where(p => friendIds.Contains(p.AppUserId))
                .OrderByDescending(p => p.DateCreated)
                .Select(p => new PostsViewModel
                {
                    Id = p.Id,
                    PostTitle = p.PostTitle,
                    PostContent = p.PostContent,
                    AppUserId = p.AppUserId,
                    Image = p.Image,
                    DateCreated = p.DateCreated,
                    CommentsCount = p.CommentsCount,
                    LikesCount = p.LikesCount,
                    Likes = p.Likes
                });
               

            return await followedPosts.ToListAsync(); 
        }





    }
}