using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    // Implementacja serwisu odpowiedzialnego za operacje na celach oszczędnościowych.
    // Wykorzystuje kontekst bazy danych ApplicationDbContext do komunikacji z bazą danych.
    public class SavingGoalsService : ISavingGoalsService
    {
        private readonly ApplicationDbContext _context;

        // Konstruktor serwisu pobiera ApplicationDbContext przez wstrzykiwanie zależności (Dependency Injection).
        public SavingGoalsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Metoda tworzy nowy cel oszczędnościowy i zapisuje go w bazie danych.
        // Zwraca utworzony obiekt z nadanym przez bazę Id.
        public async Task<SavingGoals> AddNewSavingGoalAsync(SavingGoals savingGoal)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Id == savingGoal.userId);
            if(!userExists)
            {
                throw new Exception("Użytkownik o podanym ID nie istnieje.");
            }

            var newSavingGoal = new SavingGoals
            {
                Name = savingGoal.Name,
                GoalAmount = savingGoal.GoalAmount,
                CurrentAmount = savingGoal.CurrentAmount,
                TargetDate = savingGoal.TargetDate,
                Date = DateTime.Now,        // Aktualna data utworzenia celu
                userId = savingGoal.userId
            };

            // Dodanie nowego celu do kontekstu
            await _context.SavingGoals.AddAsync(newSavingGoal);
            // Zapisanie zmian w bazie
            await _context.SaveChangesAsync();

            return newSavingGoal;
        }

        // Metoda usuwa cel oszczędnościowy na podstawie Id oraz userId. 
        // Usuwanie jest wykonywane tylko, jeśli cel zostanie znaleziony.
        public async Task DeleteSavingGoalAsync(int id, int userId)
        {
            var savingGoal = await _context.SavingGoals
                                           .FirstOrDefaultAsync(s => s.Id == id && s.userId == userId);

            if (savingGoal != null)
            {
                _context.SavingGoals.Remove(savingGoal);
                await _context.SaveChangesAsync();
            }
        }

        // Metoda zwraca listę wszystkich celów oszczędnościowych należących do danego użytkownika.
        public async Task<IEnumerable<SavingGoals>> GetAllSavingGoalsAsync(int userId)
        {
            var savingGoals = await _context.SavingGoals
                                            .Where(s => s.userId == userId)
                                            .ToListAsync();

            return savingGoals;
        }

        // Metoda zwraca postęp (w procentach) realizacji danego celu na podstawie jego Id i userId.
        // Jeśli cel nie istnieje, zwraca null.
        // Jeśli GoalAmount = 0, to zwraca 0%, aby uniknąć dzielenia przez zero.
        public async Task<double?> GetProgressAsync(int id, int userId)
        {
            var savingGoal = await _context.SavingGoals
                                           .FirstOrDefaultAsync(s => s.Id == id && s.userId == userId);

            if (savingGoal == null)
            {
                return null;
            }

            if (savingGoal.GoalAmount == 0)
            {
                return 0;
            }

            double progress = (double)savingGoal.CurrentAmount / (double)savingGoal.GoalAmount * 100.0;
            return progress;
        }

        // Metoda pobiera pojedynczy cel oszczędnościowy na podstawie Id i userId.
        // Jeśli nie znajdzie takiego celu, zwraca null.
        public async Task<SavingGoals?> GetSavingGoalByIdAsync(int id, int userId)
        {
            return await _context.SavingGoals
                                 .FirstOrDefaultAsync(s => s.Id == id && s.userId == userId);
        }

        // Metoda aktualizuje istniejący cel oszczędnościowy.
        // Jeśli nie znajdzie celu o podanym Id i userId, zwraca null.
        // W przeciwnym wypadku aktualizuje pola i zapisuje zmiany w bazie.
        public async Task<SavingGoals> UpdateSavingGoalAsync(int id, SavingGoals savingGoal, int userId)
        {
            var savingGoalToUpdate = await _context.SavingGoals
                                                   .FirstOrDefaultAsync(s => s.Id == id && s.userId == userId);

            if (savingGoalToUpdate == null)
            {
                return null; 
            }

            // Aktualizacja pól celu
            savingGoalToUpdate.Name = savingGoal.Name;
            savingGoalToUpdate.GoalAmount = savingGoal.GoalAmount;
            savingGoalToUpdate.CurrentAmount = savingGoal.CurrentAmount;
            savingGoalToUpdate.TargetDate = savingGoal.TargetDate;

            // Zapisanie zmian w bazie
            await _context.SaveChangesAsync();
            return savingGoalToUpdate;
        }
    }
}
