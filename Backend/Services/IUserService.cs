using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterDto registerDto);
        Task<Users> LoginUserAsync(LoginDto loginDto);
        string GenerateJwtToken(Users user);
    }
}