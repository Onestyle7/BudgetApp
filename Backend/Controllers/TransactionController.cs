using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        transaction.UserId = userId;

        // Dodanie transakcji za pomocą serwisu
        var newTransaction = await _transactionService.AddTransactionAsync(transaction);

        // Zwraca 200 z dodaną transakcją
        return Ok(newTransaction);
    }
}
}
