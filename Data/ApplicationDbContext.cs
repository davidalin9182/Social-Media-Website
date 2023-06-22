using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Models;

namespace Social_Media_Website.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Login> Login { get; set; }
        public DbSet<Register> Register { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friend> Friend { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<Like> Likes { get; set; }

       

    }
}
