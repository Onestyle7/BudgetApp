using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models; // Używamy modelu Transaction z Backend.Models
using Backend.Data;

namespace Backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Transactions> AddTransactionAsync(Transactions transaction)
        {
            // Dodajemy transakcję do kontekstu bazy danych za pomocą metody AddAsync().
            // Zwracany jest obiekt EntityEntry<Transactions>, który zawiera szczegóły dodanej encji.
            var transac = await _context.Transactions.AddAsync(transaction);

            // Zapisujemy zmiany w bazie danych.
            // Jeśli operacja zapisu się nie powiedzie, transakcja nie zostanie dodana.
            await _context.SaveChangesAsync();
            
            // Sprawdzamy, czy dodanie transakcji powiodło się.
            // Jeśli obiekt transac jest null, zgłaszamy wyjątek.
            if (transac == null) throw new Exception("Transaction not added");

            // Zwracamy właściwy obiekt Transactions z EntityEntry.
            // Właściwość Entity zawiera oryginalny obiekt transakcji, który został dodany.
            return transac.Entity; // Zwracamy transakcję z właściwości Entity
        }

        public Task<Transactions> DeleteTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transactions>> GetAllTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Transactions> GetTransactionByIdAsync(int id)
        {
            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(transaction);
        }

        public Task<Transactions> UpdateTransactionAsync(int id, Transactions transaction)
        {
            throw new NotImplementedException();
        }
    }
}
