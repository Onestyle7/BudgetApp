using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;
        
        public UserService(JwtSettings jwtSettings, ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings;
            _context = context;
        }
        public string GenerateJwtToken(Users user){
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.LoginData.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Users> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.LoginData)
                .FirstOrDefaultAsync(u => u.LoginData.Email == loginDto.Email);

            if (user == null) return null; 

            if(!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.LoginData.Password))
                return null;

            return user;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            var existingUser = await _context.Users
                .Include(u => u.LoginData)
                .FirstOrDefaultAsync(u => u.LoginData.Email == registerDto.Email);

            if(existingUser != null){
                return false;
            }
            var user = new Users{
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                LoginData = new LoginData{
                    Email = registerDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}