using AutoMapper;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using BCrypt.Net;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mówimy AutoMapperowi, że chcemy mapować RegisterDto na Users
        CreateMap<RegisterDto, Users>()
            // Ustawiamy pole LoginData w Users na podstawie danych z RegisterDto
            .ForMember(dest => dest.LoginData, opt => opt.MapFrom(src => new LoginData
            {
                Email = src.Email, // Email ustawiamy na podstawie pola Email z RegisterDto
                Password = HashPassword(src.Password) // Hasło jest hashowane i zapisane
            }))
            // Możemy dodać inne mapowania, jeśli będą potrzebne
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Ustawiamy datę rejestracji na aktualną datę
    }

    // Metoda do hashowania haseł (z użyciem BCrypt)
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password); // Zwraca hasło w formie zaszyfrowanej
    }
}