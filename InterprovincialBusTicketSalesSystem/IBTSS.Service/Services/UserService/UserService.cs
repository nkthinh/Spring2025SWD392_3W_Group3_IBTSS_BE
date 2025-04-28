using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Response.User;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IBTSS.Service.DTO.Request.Trip;

namespace IBTSS.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddUserAsync(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);

            //// Không cho tạo Admin account mới
            //if (user.Role == UserRole.Admin)
            //    throw new Exception("Cannot create another Admin account.");

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.Where(u => u.Role != UserRole.Admin); 
        }


        public async Task<User?> GetByIdAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.Role == UserRole.Admin) return null; //Không được lấy Admin
            return user;
        }


        public User? GetByUsername(string username)
        {
            var user = _unitOfWork.Users.GetByUsername(username);
            if (user == null || user.Role == UserRole.Admin) return null; //Không được lấy Admin
            return user;
        }


        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.UserId);
            if (existingUser == null || existingUser.Role == UserRole.Admin)
                throw new Exception("Cannot update Admin account.");

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = HashPassword(user.PasswordHash);
            }

            // ❗ Không cho phép thay đổi Role thành Admin
            if (user.Role == UserRole.Admin)
                throw new Exception("Cannot update role to Admin.");

            await _unitOfWork.Users.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteUserAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.Role == UserRole.Admin)
                throw new Exception("Cannot delete Admin account.");

            user.IsDelete = true;
            await _unitOfWork.Users.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();
        }

        public User? Authenticate(string username, string password)
        {
            var user = _unitOfWork.Users.GetByUsername(username);
            if (user == null || user.IsDelete) return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
        }


        public async Task<(List<AddUserResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var filtered = users.Where(u => u.Role != UserRole.Admin).AsQueryable(); // ❗ Không lấy Admin

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(u =>
                    (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(query.Keyword))
                );
            }

            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = _mapper.Map<List<AddUserResponse>>(filtered.ToList());
                return (allMapped, allMapped.Count);
            }

            var result = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = _mapper.Map<List<AddUserResponse>>(result);
            return (mapped, total);
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
