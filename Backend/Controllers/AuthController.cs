using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.LoginUserAsync(loginDto);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _userService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _userService.RegisterUserAsync(registerDto);

            if (!result)
            {
                return BadRequest("Email already in use");
            }

            return Ok("User registered successfully");
        }
    }
}
