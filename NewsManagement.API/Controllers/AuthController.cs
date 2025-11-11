using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;
using System.Security.Claims;

namespace NewsManagement.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest(ApiResponse<string>.Fail("Email already exists", 400));

            return Ok(ApiResponse<AuthResponse>.Success(result, "Registration successful"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid email or password", 401));

            return Ok(ApiResponse<AuthResponse>.Success(result, "Login successful"));
        }
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        userId,
                        email,
                        role = GetRoleName(role),
                       
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private string GetRoleName(string roleId)
        {
            return roleId switch
            {
                "1" => "Staff",
                "2" => "Lecturer",
                "3" => "Admin",
                _ => "Unknown"
            };
        }
    }
}
