using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Data;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Social_Media_Website.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly string _connectionString;
        public UserRepository(ApplicationDbContext context, UserManager<AppUser> userManager, string connectionString)
        {
            _context = context;
            _userManager = userManager;
            _connectionString = connectionString;
        }

        public bool Add(AppUser user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<List<AppUser>> GetAllUsers2()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersByLocation(string country, string city)
        {
            return await _context.Users
                .Where(u => u.Country == country && u.City == city)
                .ToListAsync();
        }

        public async Task<IEnumerable<Posts>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }
        public async Task<List<Posts>> GetUserPosts(string userId)
        {
            var userPosts = _context.Posts.Where(r => r.AppUser.Id == userId);
            return await userPosts.ToListAsync();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }
        
        public async Task<IEnumerable<AppUser>> GetAllUsersExcept(IEnumerable<string> friendIds)
        {
            var users = await _userManager.Users.Where(u => !friendIds.Contains(u.Id)).ToListAsync();
            return users;
        }

        public async Task<IEnumerable<AppUser>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return await _context.Users.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
        public async Task<IEnumerable<AppUser>> GetUserByName(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM AspNetUsers WHERE UserName LIKE '%' + @UserName + '%'";
                var parameters = new { UserName = userName };
                var users = await connection.QueryAsync<AppUser>(query, parameters);
                return users;
            }
        }
        public async Task<List<AppUser>> GetUserByName2(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM AspNetUsers WHERE UserName LIKE '%' + @UserName + '%'";
                var parameters = new { UserName = userName };
                var users = await connection.QueryAsync<AppUser>(query, parameters);
                var userslist = users.ToList();
                return userslist;
            }
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}