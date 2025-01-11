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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();
                return BadRequest(new { message = "Błędne dane wejściowe.", errors });
            }

            try
            {
                var user = await _userService.LoginUserAsync(loginDto);

                if (user == null)
                {
                    return Unauthorized(new { message = "Nieprawidłowe dane logowania." });
                }

                var token = _userService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wewnętrzny błąd serwera", error = ex.Message });
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerDto);

                if (!result)
                {
                    return BadRequest(new {message = "Email already in use"});
                }

                return Ok(new{message = "User registered successfully"});
            }
            
            catch(Exception ex)
            {
                return StatusCode(500, new {message = "Wewnętrzny błąd serwera", error = ex.Message});
            }
           
        }
        
    }
}
