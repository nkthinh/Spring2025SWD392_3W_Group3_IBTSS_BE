using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.UserRepository;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash); // Mã hóa mật khẩu
            await _userRepository.AddUserAsync(user); // ✅ dùng await
        }

        public User? Authenticate(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null || user.IsDelete) return null;

            var hash = HashPassword(password);
            if (user.PasswordHash != hash) return null;

            return user;
        }

        public User? GetByUsername(string username)
        {
            return _userRepository.GetByUsername(username);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
