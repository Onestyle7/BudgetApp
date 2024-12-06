using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Backend.Models.Enums;

namespace Backend.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        [Required]
        [Range(0.01, Double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Category Category { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
    }
}