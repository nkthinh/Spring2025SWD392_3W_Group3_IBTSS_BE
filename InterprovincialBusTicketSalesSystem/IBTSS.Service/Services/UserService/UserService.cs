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
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _unitOfWork.Users.GetByIdAsync(userId);
        }

        public User? GetByUsername(string username)
        {
            return _unitOfWork.Users.GetByUsername(username);
        }

        public async Task UpdateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = HashPassword(user.PasswordHash);
            }

            await _unitOfWork.Users.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteUserAsync(string userId)
        {
            await _unitOfWork.Users.DeleteUserAsync(userId);
            await _unitOfWork.CompleteAsync();
        }

        public User? Authenticate(string username, string password)
        {
            var user = GetByUsername(username);
            if (user == null || user.IsDelete) return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
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
