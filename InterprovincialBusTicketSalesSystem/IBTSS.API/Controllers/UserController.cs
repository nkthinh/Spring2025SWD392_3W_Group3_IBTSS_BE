using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Service.DTO.Request.User;
using IBTSS.Service.DTO.Response.User;
using IBTSS.Service.Services.JWT;
using IBTSS.Service.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IMapper mapper, JwtService jwtService) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserRequest request)
        {
            try
            {
                var user = userService.Authenticate(request.Username, request.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = jwtService.GenerateToken(user);
                var userResponse = mapper.Map<LoginUserResponse>(user);

                return Ok(new
                {
                    User = userResponse,
                    Token = token,
                  
                   
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUserRequest request)
        {
            try
            {
                var existingUser = userService.GetByUsername(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists." });
                }

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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
