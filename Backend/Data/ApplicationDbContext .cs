using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _options = options;
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<LoginData> LoginData { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<SavingGoals> SavingGoals { get; set; }

    }
}