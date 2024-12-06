using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models.Enums;

namespace Backend.DTOs
{
    public class CreateTransactionDto
{
    [Required] // Pole jest wymagane
    [Range(0.01, Double.MaxValue)] // Wartość musi być większa niż 0.01
    public decimal Amount { get; set; } // Kwota transakcji

    [Required] // Pole jest wymagane
    public DateTime Date { get; set; } // Data transakcji

    [Required] // Pole jest wymagane
    public Category category { get; set; } // Kategoria transakcji (np. jedzenie, transport)
}

}