using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; } // String dla nazwy kategorii
        public string? Name { get; set; }
        public required string UserName { get; set; } // Imię i nazwisko użytkownika
        public string Name { get; set; }

    

    }

}