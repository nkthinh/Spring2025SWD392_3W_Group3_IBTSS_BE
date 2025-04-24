using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.User;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<(List<LoginUserResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var filtered = customers.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(query.Keyword))
                );
            }

            filtered = query.SortBy switch
            {
                "name_desc" => filtered.OrderByDescending(c => c.Name),
                "score_desc" => filtered.OrderByDescending(c => c.Score),
                "DiscountQuotaLeft_desc" => filtered.OrderByDescending(c => c.DiscountQuotaLeft),
                _ => filtered.OrderBy(c => c.Name)
            };

            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = _mapper.Map<List<LoginUserResponse>>(filtered.ToList());
                return (allMapped, allMapped.Count);
            }

            var result = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = _mapper.Map<List<LoginUserResponse>>(result);
            return (mapped, total);
        }
    }
}
