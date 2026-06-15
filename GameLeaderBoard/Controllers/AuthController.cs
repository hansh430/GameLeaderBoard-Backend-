using GameLeaderBoard.Data;
using GameLeaderBoard.Dtos;
using GameLeaderBoard.Models;
using GameLeaderBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace GameLeaderBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _gameDbContext;
        private readonly IJwtService _jwt;

        public AuthController(GameDbContext gameDbContext, IJwtService jwt)
        {
            _gameDbContext = gameDbContext;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _gameDbContext.Users.Add(user);
            await _gameDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginDto dto)
        {
            var user = await _gameDbContext.Users.FirstOrDefaultAsync(x=>x.Username == dto.Username);
            if (user == null)
            {
                return Unauthorized();
            }
            var valid =BCrypt.Net.BCrypt.Verify(dto.Password,user.Password);
            if (!valid)
                return Unauthorized();

            var token = _jwt.GenerateToken(user);
            return Ok(new
            {
                Token = token
            });
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
                return Unauthorized();

            _gameDbContext.RevokedTokens.Add(new RevokedToken
            {
                JwtId = jti,
                RevokedAt = DateTime.UtcNow
            });

            await _gameDbContext.SaveChangesAsync();

            return Ok("Logged out successfully");
        }

    }
}
