using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    // Interfejs serwisu dla operacji na celach oszczędnościowych.
    // Definiuje metody, które musi zrealizować klasa implementująca ten interfejs.
    public interface ISavingGoalsService
    {
        // Dodaje nowy cel oszczędnościowy i zwraca go po zapisaniu w bazie.
        Task<SavingGoals> AddNewSavingGoalAsync(SavingGoals savingGoal);

        // Zwraca wszystkie cele oszczędnościowe dla danego użytkownika.
        Task<IEnumerable<SavingGoals>> GetAllSavingGoalsAsync(int userId);

        // Zwraca cel oszczędnościowy po Id i userId, lub null jeśli nie istnieje.
        Task<SavingGoals?> GetSavingGoalByIdAsync(int id, int userId);

        // Aktualizuje istniejący cel oszczędnościowy i zwraca zaktualizowany obiekt.
        // Jeśli cel nie istnieje, zwraca null.
        Task<SavingGoals> UpdateSavingGoalAsync(int id, SavingGoals savingGoal, int userId);

        // Usuwa cel oszczędnościowy o podanym Id i należący do danego użytkownika.
        Task DeleteSavingGoalAsync(int id, int userId);

        // Zwraca procentowy postęp realizacji celu (double?).
        // Jeśli nie ma celu, zwraca null.
        Task<double?> GetProgressAsync(int id, int userId);
    }
}
