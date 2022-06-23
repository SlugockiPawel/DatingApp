using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repos
{
    public class PostgresAppUserRepo : IAppUserRepo
    {
        private readonly ApplicationDbContext _context;

        public PostgresAppUserRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id.Equals(id));
        }
    }
}
