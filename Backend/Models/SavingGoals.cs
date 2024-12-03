using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class SavingGoals
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Range(0.01, Double.MaxValue)]
        public decimal GoalAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        [Required]
        public DateTime TargetDate { get; set; }        
        public DateTime Date { get; set; }
        public int userId { get; set; }
        public Users User { get; set; }
    }
}