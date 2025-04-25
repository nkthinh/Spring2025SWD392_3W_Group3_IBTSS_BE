using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Request.User;
using IBTSS.Service.DTO.Response.User;
using IBTSS.Service.Services.JWT;
using IBTSS.Service.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IMapper mapper, JwtService jwtService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters query)
        {
            try
            {
                // Nếu không có tham số query, trả về tất cả người dùng
                if (query == null ||
                    (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var users = await userService.GetAllAsync();
                    var mapped = mapper.Map<List<AddUserResponse>>(users);
                    return Ok(mapped);
                }

                // Nếu không có giá trị cho Page và PageSize, mặc định là Page = 1 và PageSize = 10
                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                // Lấy dữ liệu đã lọc và tổng số bản ghi từ service
                var (data, totalCount) = await userService.GetFilteredAsync(query);

                // Tính toán số trang tổng cộng
                var pagination = new
                {
                    TotalCount = totalCount,
                    PageSize = query.PageSize,
                    CurrentPage = query.Page,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                };

                // Trả về dữ liệu và thông tin phân trang
                return Ok(new
                {
                    Data = data,
                    Pagination = pagination
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //admin only
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with ID '{id}' not found." });

                var response = mapper.Map<AddUserResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AddUserRequest request)
        {
            try
            {
                var user = await userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with ID '{id}' not found." });

                user.Name = request.Name;
                user.PhoneNumber = request.PhoneNumber;
                user.Role = request.Role;
                user.PasswordHash = request.Password;

                await userService.UpdateUserAsync(user);
                return Ok(mapper.Map<AddUserResponse>(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with ID '{id}' not found." });

                await userService.DeleteUserAsync(id);
                return Ok(new { message = "User deleted (soft delete) successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserRequest request)
        {
            try
            {
                var user = userService.Authenticate(request.Username, request.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password." });

                var token = jwtService.GenerateToken(user);
                var userResponse = mapper.Map<LoginUserResponse>(user);

                return Ok(new
                {
                    user = userResponse,
                    token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUserRequest request)
        {
            try
            {
                var existingUser = userService.GetByUsername(request.Username);
                if (existingUser != null)
                    return BadRequest(new { message = "Username already exists." });

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = request.Password,
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role == 0 ? UserRole.Staff : request.Role
                };

                await userService.AddUserAsync(user);
                var response = mapper.Map<AddUserResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
