using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public class LoginRequest
        {
            public string Login { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = null!;
            public string Role { get; set; } = null!;
        }

        /// <summary>
        /// јутентификаци€ пользовател€ и получение JWT токена.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByLoginAsync(request.Login);
            if (user == null || user.Password != request.Password || user.RevokedOn != null)
                return Unauthorized("Ќеверный логин или пароль, либо пользователь заблокирован.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new LoginResponse
            {
                Token = token,
                Role = user.Admin ? "Admin" : "User"
            });
        }
    }
}
