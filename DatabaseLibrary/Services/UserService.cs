using DatabaseLibrary.Data;
using DatabaseLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLibrary.Services
{
    public class UserService
    {
        private readonly ShopContext _context = new();

        public async Task<bool> IsUserExist(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            return user != null;
        }

        public async Task<User?> GetUserByLogin(string login)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<string?> GetUserRole(string login)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);

            return user?.Role?.Name;
        }

        public async Task<User?> GetUserWithRole(string login, string password)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
        }
    }
}