using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddUserAsync(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.CompleteAsync(); // thêm nếu muốn commit DB luôn
        }

        public User? Authenticate(string username, string password)
        {
            var user = _unitOfWork.Users.GetByUsername(username);
            if (user == null || user.IsDelete) return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
        }

        public User? GetByUsername(string username)
        {
            return _unitOfWork.Users.GetByUsername(username);
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
