using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            var lastUser = await _context.Users.OrderByDescending(u => u.UserId).FirstOrDefaultAsync();
            string newId = "U001";

            if (lastUser != null)
            {
                int number = int.Parse(lastUser.UserId.Substring(1));
                newId = "U" + (++number).ToString("D3");
            }

            user.UserId = newId;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.IsDelete = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

      
    }
}
