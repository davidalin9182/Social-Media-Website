using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Data;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;

namespace Social_Media_Website.Repository
{
    public class AboutRepository : IAboutRepository
    {
        private readonly ApplicationDbContext _context;

        public AboutRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        

    }
}