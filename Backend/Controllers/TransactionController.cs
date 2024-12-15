using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize] // Endpointy w tym kontrolerze wymagają autoryzacji (użytkownik musi być zalogowany)
    [ApiController] // Informuje, że klasa jest kontrolerem API
    [Route("api/[controller]")] // Trasa kontrolera: api/Transaction
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        // Konstruktor wstrzykuje zależności: serwis transakcji i AutoMapper
        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpGet("{id}")] // Trasa: GET api/Transaction/{id}
        public async Task<ActionResult<Transactions>> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id); // Pobranie transakcji z serwisu
            if (transaction == null)
            {
                return NotFound(); // Zwraca 404, jeśli transakcja nie została znaleziona
            }
            return Ok(transaction); // Zwraca 200 z transakcją w treści odpowiedzi
        }
        [HttpGet("all")] // Trasa: GET api/Transaction/all
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactionsAsync(
            [FromQuery] string sortBy = "date",
            [FromQuery] bool descending = false,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null         
             )
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); // Pobranie ID zalogowanego użytkownika
            var transactions = await _transactionService.GetAllTransactionsAsync(userId); // Pobranie wszystkich transakcji

            // Filtrowanie po kategorii jeśli jest podana
            if(!string.IsNullOrEmpty(category)){
                transactions = transactions.Where(t => string.Equals(t.Category.ToString(), category, StringComparison.OrdinalIgnoreCase));
            }
            // Filtrowanie po minAmount i maxAmount, jeśli są podane
            if(minAmount.HasValue){
                transactions = transactions.Where(t => t.Amount >= minAmount);   
            }

            if(maxAmount.HasValue){
                transactions = transactions.Where(t => t.Amount <= maxAmount);
            } 

            // Filtrowanie po dacie 
            if(startDate.HasValue){
                transactions = transactions.Where(t => t.Date >= startDate);
            }

            if(endDate.HasValue){
                transactions = transactions.Where(t => t.Date <= endDate);
            }

            //Sortowanie wg parametry sortBy

            transactions = sortBy.ToLower() switch{
                "category" => descending ? transactions.OrderByDescending(t => t.Category) : transactions.OrderBy(t => t.Category),
                "amount" => descending ? transactions.OrderByDescending(t => t.Amount) : transactions.OrderBy(t => t.Amount),

                //Domyślnie sortujemy po dacie

                _ => descending ? transactions.OrderByDescending(t => t.Date) : transactions.OrderBy(t => t.Date)
            };

            var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions); // Mapowanie modelu Transactions na DTO
            return Ok(transactionDtos); 
        }

        [HttpPost("add")] // Trasa: POST api/Transaction/add
        public async Task<ActionResult<Transactions>> AddTransactionAsync([FromBody] CreateTransactionDto createTransactionDto)
        {
            if (!ModelState.IsValid) // Walidacja danych wejściowych
            {
                return BadRequest("Invalid transaction data"); // Zwraca 400 w przypadku błędnych danych
            }

            // Mapowanie DTO na model Transactions za pomocą AutoMapper
            var transaction = _mapper.Map<Transactions>(createTransactionDto);

            
            
            // Pobieranie ID zalogowanego użytkownika z tokenu JWT
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            transaction.UserId = userId;

            // Dodanie transakcji za pomocą serwisu
            var newTransaction = await _transactionService.AddTransactionAsync(transaction);

            // Zwraca 200 z dodaną transakcją
            return Ok(newTransaction);
        }
        [HttpDelete("{id}")] // Trasa: DELETE api/Transaction/{id}
        public async Task<ActionResult> DeleteTransactionAsync(int id){
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); // Pobranie ID zalogowanego użytkownika
            await _transactionService.DeleteTransactionAsync(id, userId); // Usunięcie transakcji
            return Ok(); // Zwraca 200 w przypadku sukcesu
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message); // Zwraca 403 Forbidden
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Zwraca 400 w przypadku innych błędów
        }
}

        [HttpPut("{id}")] // Trasa: PUT api/Transaction/{id}
        public async Task<ActionResult<Transaction>> UpdateTransactionAsync(int id, [FromBody] CreateTransactionDto createTransactionDto)
        {
            if(!ModelState.IsValid) // Walidacja danych wejściowych
            {
                return BadRequest("Invalid transaction data"); // Zwraca 400 w przypadku błędnych danych
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); // Pobranie ID zalogowanego użytkownika
            var transaction = _mapper.Map<Transactions>(createTransactionDto); // Mapowanie DTO na model Transactions
            try
            {
                var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transaction, userId);
                return Ok(updatedTransaction);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message); // Zwraca 403 Forbidden
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // Zwraca 400 w przypadku innych błędów
            }
        }
    }
}
