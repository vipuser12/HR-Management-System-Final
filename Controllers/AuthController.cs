using HRManagementSys.Data;
using HRManagementSys.Dtos;
using HRManagementSys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HRManagementSys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                
                var hashedPassword = HashPassword(loginDto.Password);
                
                if (user.PasswordHash != hashedPassword)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = await _tokenService.GenerateTokenAsync(user);
                var permissions = await _tokenService.GetUserPermissionsAsync(user.Id);

                var response = new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
           
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
