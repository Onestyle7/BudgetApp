using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavingGoalsController : ControllerBase
    {
        private readonly ISavingGoalsService _savingGoalsService;

        // Konstruktor przyjmuje interfejs serwisu zamiast konkretnej implementacji.
        // Dzięki temu łatwiej testować kontroler (dependency injection, testy jednostkowe).
        public SavingGoalsController(ISavingGoalsService savingGoalsService)
        {
            _savingGoalsService = savingGoalsService;
        }

        /// Zwraca wszystkie cele oszczędnościowe danego użytkownika.
        /// Wymaga autoryzacji oraz identyfikatora użytkownika.
        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<SavingGoals>> AddNewSavingGoalAsync([FromBody] AddSavingGoalDto dto)
        {
            // Pobranie userId z claimów zalogowanego użytkownika
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Brak userId w claimach.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Nie można przetworzyć userId z claimów.");
            }

            // Tworzymy obiekt domenowy, który przekażemy do serwisu
            var savingGoal = new SavingGoals
            {
                Name = dto.Name,
                GoalAmount = dto.GoalAmount,
                CurrentAmount = dto.CurrentAmount,
                TargetDate = dto.TargetDate,
                Date = DateTime.Now,
                userId = userId
            };
            
            var newSavingGoal = await _savingGoalsService.AddNewSavingGoalAsync(savingGoal);

            return Ok(newSavingGoal);
        }
    

        /// Zwraca pojedynczy cel oszczędnościowy na podstawie ID i userId.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpGet("{id}")]
        public async Task<SavingGoals?> GetSavingGoalByIdAsync(int id, [FromQuery] int userId)
        {
            var savingGoal = await _savingGoalsService.GetSavingGoalByIdAsync(id, userId);
            return savingGoal;
        }

        /// Dodaje nowy cel oszczędnościowy dla użytkownika.
        /// Zakładamy, że obiekt SavingGoals zawiera w sobie userId lub zostanie on przekazany wraz z żądaniem.
        /// Wymaga autoryzacji.
        

        /// Aktualizuje istniejący cel oszczędnościowy na podstawie ID i userId.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<SavingGoals> UpdateSavingGoalAsync(int id, [FromBody] SavingGoals savingGoal, [FromQuery] int userId)
        {
            var updatedSavingGoal = await _savingGoalsService.UpdateSavingGoalAsync(id, savingGoal, userId);
            return updatedSavingGoal;
        }

        /// Usuwa cel oszczędnościowy na podstawie ID i userId.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task DeleteSavingGoalAsync(int id, [FromQuery] int userId)
        {
            await _savingGoalsService.DeleteSavingGoalAsync(id, userId);
        }
        
        /// Zwraca postęp (w procentach) realizacji celu oszczędnościowego.
        /// Zwracane jest double? - jeżeli cel nie istnieje, to wartość może być null.
        /// Wymaga autoryzacji.
        
        [Authorize]
        [HttpGet("progress/{id}")]
        public async Task<double?> GetProgressAsync(int id, [FromQuery] int userId)
        {
            var progress = await _savingGoalsService.GetProgressAsync(id, userId);
            return progress;
        }

    }
}
