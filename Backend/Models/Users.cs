using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public DateTime RegistrationDate { get; set; }
        public LoginData LoginData { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
        public ICollection<SavingGoals> SavingGoals { get; set; }
        public Users(){
            FirstName = string.Empty;
            LastName = string.Empty;
            LoginData = new LoginData();
            Transactions = new List<Transactions>();
            SavingGoals = new List<SavingGoals>();
        }
    }
}