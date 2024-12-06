using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
  // Definicja metod do obsługi transakcji
public interface ITransactionService
{
    Task<Transactions> AddTransactionAsync(Transactions transaction); // Dodawanie transakcji
    Task<IEnumerable<Transactions>> GetAllTransactionsAsync(); // Pobranie wszystkich transakcji
    Task<Transactions> GetTransactionByIdAsync(int id); // Pobranie transakcji po ID
    Task<Transactions> UpdateTransactionAsync(int id, Transactions transaction); // Aktualizacja transakcji
    Task<Transactions> DeleteTransactionAsync(int id); // Usunięcie transakcji
}

}