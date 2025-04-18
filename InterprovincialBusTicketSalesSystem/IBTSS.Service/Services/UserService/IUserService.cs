using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.UserService
{
    public interface IUserService
    {
        Task AddUserAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string userId);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
        User? GetByUsername(string username);
        User? Authenticate(string username, string password);
    }
}
