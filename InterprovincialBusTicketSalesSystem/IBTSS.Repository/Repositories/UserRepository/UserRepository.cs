using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.UserRepository
{
    public class UserRepository: IUserRepository
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
        public async Task AddUserAsync(User user)
        {
            // Tìm UserId lớn nhất (theo định dạng U001, U002, ...)
            var lastUser = await _context.Users
                .OrderByDescending(u => u.UserId)
                .FirstOrDefaultAsync();

            string newId = "U001";

            if (lastUser != null)
            {
                string lastId = lastUser.UserId; // ví dụ: "U005"
                int number = int.Parse(lastId.Substring(1)); // bỏ "U" => 5
                number++;
                newId = "U" + number.ToString("D3"); // "U006"
            }

            user.UserId = newId;

            await _context.Users.AddAsync(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }
    }
}
