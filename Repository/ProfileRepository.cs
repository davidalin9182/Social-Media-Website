using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Social_Media_Website.Data;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;
using X.PagedList;


namespace Social_Media_Website.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        //public async Task<List<Posts>> GetAllUserPosts()
        //{
        //    var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        //    var userPosts = _context.Posts.Where(r => r.AppUser.Id == currentUserId);
        //    return userPosts.ToList();
        //}

        //public async Task<IPagedList<Posts>> GetAllUserPosts(int pageNumber, int pageSize)
        //{
        //    var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        //    var userPosts = _context.Posts .Where(r => r.AppUser.Id == currentUserId).OrderByDescending(p => p.DateCreated);

        //    return await userPosts.ToPagedListAsync(pageNumber, pageSize);
        //}

        //public async Task<IPagedList<PostsViewModel>> GetAllUserPosts(int pageNumber, int pageSize)
        //{
        //    var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

        //    var userPosts = _context.Posts
        //        .Where(r => r.AppUser.Id == currentUserId)
        //        .OrderByDescending(p => p.DateCreated)
        //        .Select(p => new PostsViewModel
        //        {
        //            Id = p.Id,
        //            PostTitle = p.PostTitle,
        //            PostContent = p.PostContent,
        //            Image = p.Image,
        //            CommentsCount = p.CommentsCount,
        //            LikesCount = p.LikesCount,
        //            Likes = p.Likes
        //        });

        //    return await userPosts.ToPagedListAsync(pageNumber, pageSize);
        //}

        public async Task<ICollection<PostsViewModel>> GetAllUserPosts()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

            var userPosts = _context.Posts
                .Where(r => r.AppUser.Id == currentUserId)
                .OrderByDescending(p => p.DateCreated)
                .Select(p => new PostsViewModel
                {
                    Id = p.Id,
                    PostTitle = p.PostTitle,
                    PostContent = p.PostContent,
                    AppUserId = p.AppUserId,
                    DateCreated = p.DateCreated,
                    Image = p.Image,
                    CommentsCount = p.CommentsCount,
                    LikesCount = p.LikesCount,
                    Likes = p.Likes
                });

            return await userPosts.ToListAsync();
        }



        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        //public AppUser GetUserById(string id)
        //{
        //    var user = _context.Users.Find(id);
        //    return user ?? null;
        //}

        public async Task<AppUser> GetByIdNoTracking(string id)
        {
            return await _context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
            
        }
        //To list like with post i can use at friends list to return more than 1 user this is for only 1 user
        //ID: d00b16db-6a00-4c54-b6f3-b958d717a9de
        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
