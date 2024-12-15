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
    

        /// Zwraca wszystkie cele oszczędnościowe zalogowanego użytkownika.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SavingGoals?>>> GetAllSavingGoalByIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim == null){
                return Unauthorized("Brak ważnego userId w claimach");
            }

            if(!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Nie można przetworzyć userId z claimów.");
            }
            // Wywołanie serwisu, aby pobrać wszystkie cele oszczędnościowe dla tego userId
            var savingGoals = await _savingGoalsService.GetAllSavingGoalsAsync(userId);
            return Ok(savingGoals);
        }
        

        /// Aktualizuje istniejący cel oszczędnościowy na podstawie ID i userId.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<SavingGoals>> UpdateSavingGoalAsync(int id, AddSavingGoalDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Brak ważnego userId w claimach.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Nie można przetworzyć userId z claimów.");
            }

            var savingGoal = new SavingGoals
            {
                Name = dto.Name,
                GoalAmount = dto.GoalAmount,
                CurrentAmount = dto.CurrentAmount,
                TargetDate = dto.TargetDate,
                Date = DateTime.Now,
                userId = userId
            };

            var updatedSavingGoal = await _savingGoalsService.UpdateSavingGoalAsync(id, savingGoal, userId);
            if (updatedSavingGoal == null)
            {
                return NotFound("Nie znaleziono celu do aktualizacji.");
            }
            return Ok(updatedSavingGoal);
        }


        /// Usuwa cel oszczędnościowy na podstawie ID i userId.
        /// Wymaga autoryzacji.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSavingGoalAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim == null)
            {
                return Unauthorized("Brak ważnego userId w claimach");
            }    

            if(!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Nie można przetworzyć userId z claimów.");
            }

            // Wywołanie serwisu w celu usunięcia celu oszczeędnościowego
            await _savingGoalsService.DeleteSavingGoalAsync(id, userId);
            
            return NoContent(); //Zwraca 204 no Content w przypadku sukcesu
        }
        
        /// Zwraca postęp (w procentach) realizacji celu oszczędnościowego.
        /// Zwracane jest double? - jeżeli cel nie istnieje, to wartość może być null.
        /// Wymaga autoryzacji.
        
        [Authorize]
        [HttpGet("progress/{id}")]
        public async Task<ActionResult<double?>> GetProgressAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim == null)
            {
                return Unauthorized("Brak ważnego userId w claimach");
            }

            if(!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Nie można przetworzyć userId z claimów.");
            }

            //Pobranie postępu realizacji celu 
            var progress = await _savingGoalsService.GetProgressAsync(id, userId);
            if(progress == null)
            {
                return NotFound("Nie znaleziono celu.");
            }

            return Ok(progress);
        }

    }
}
