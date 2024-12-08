using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models; // Używamy modelu Transaction z Backend.Models
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        // Usuwanie transakcji przypisanej do konkretnego użytkownika
        public async Task DeleteTransactionAsync(int id, int userId)
        {
            // Znajduje transakcję na podstawie ID i ID użytkownika
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            // Sprawdza, czy transakcja istnieje i czy użytkownik ma uprawnienia do jej usunięcia
            if (transaction == null)
            {
                throw new UnauthorizedAccessException("TTransaction not found or you are not allowed to delete it");
            }

            // Usuwa transakcję z bazy danych
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        // Pobieranie wszystkich transakcji przypisanych do konkretnego użytkownika
        public async Task<IEnumerable<Transactions>> GetAllTransactionsAsync(int userId)
        {
            // Filtruje transakcje w bazie danych na podstawie ID użytkownika i dołącza dane użytkownika
            return await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.User)
            .ToListAsync();
        }
        // Pobieranie transakcji na podstawie ID (bez sprawdzania użytkownika)
        public Task<Transactions?> GetTransactionByIdAsync(int id)
        {
            // Znajduje transakcję w bazie danych na podstawie ID
            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(transaction);
        }
        // Aktualizowanie transakcji przypisanej do konkretnego użytkownika
        public async Task<Transactions> UpdateTransactionAsync(int id, Transactions transaction, int userId)
        {
            // Znajdujemy istniejącą transakcję
            var existingTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
            // Sprawdza, czy transakcja istnieje i czy użytkownik ma uprawnienia do jej aktualizacji
            if (existingTransaction == null)
            {
                throw new UnauthorizedAccessException("Transaction not found or you are not allowed to update it");
            }

            // Aktualizujemy pola transakcji
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Category = transaction.Category;
            existingTransaction.Date = transaction.Date;

            // Zapisujemy zmiany
            await _context.SaveChangesAsync();

            return existingTransaction;
        }

    }
}
