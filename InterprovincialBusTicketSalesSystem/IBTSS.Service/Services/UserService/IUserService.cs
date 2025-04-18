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
        User? Authenticate(string username, string password);
        Task AddUserAsync(User user);
        User? GetByUsername(string username);
    }
}
