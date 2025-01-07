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
            // Podstawowe informacje o użytkowniku (ID, email, name)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.LoginData.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            // Klucz podpisujący i algorytm
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
             // Tworzenie tokenu JWT
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Zwrócenie tokenu jako string
        }
        // Logowanie użytkownika na podstawie emaila i hasła
        public async Task<Users?> LoginUserAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.LoginData)
                    .FirstOrDefaultAsync(u => u.LoginData.Email == loginDto.Email);

                if (user == null)
                {
                    return null; // Zwróć null zamiast rzucania wyjątku
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.LoginData.Password))
                {
                    return null; // Zwróć null w przypadku błędnych danych
                }

                return user; // Zwróć użytkownika, jeśli wszystko się zgadza
            }
            catch (Exception ex)
            {
                // Loguj szczegóły błędu
                Console.WriteLine($"Błąd logowania: {ex.Message}");
                throw; // Przekaż dalej w razie poważnych błędów
            }
        }
        // Rejestracja nowego użytkownika
        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            // Sprawdzenie, czy użytkownik z podanym emailem już istnieje
            var existingUser = await _context.Users
                .Include(u => u.LoginData)
                .FirstOrDefaultAsync(u => u.LoginData.Email == registerDto.Email);

            if(existingUser != null){
                return false; // Email już zajęty
            }
            // Tworzenie nowego użytkownika
            var user = new Users{
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                LoginData = new LoginData{
                    Email = registerDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password) // Haszowanie hasła
                }
            };
            // Dodanie użytkownika do bazy danych
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true; // Rejestracja zakończona sukcesem
        }
    }
}