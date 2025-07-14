using Expense.Application.DTOs;
using Expense.Domain.Entities;
using Expense.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Expense.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExpenseController(ExpenseDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateExpenseDto dto)
        {
            var expense = new ExpenseItem
            {
                UserId = dto.UserId,
                Category = dto.Category,
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // 1. Get current month budget from BudgetService
            var httpClient = _httpClientFactory.CreateClient();
            var budgetApiUrl = $"http://localhost:5082/api/budget/user/{dto.UserId}/current";

            HttpResponseMessage budgetResponse;
            try
            {
                budgetResponse = await httpClient.GetAsync(budgetApiUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("BudgetService unreachable: " + ex.Message);
                return Ok("Expense saved, but budget check failed.");
            }

            if (!budgetResponse.IsSuccessStatusCode)
                return Ok("Expense saved, but no budget found for this month.");

            var budgetJson = await budgetResponse.Content.ReadAsStringAsync();
            dynamic budget = JsonConvert.DeserializeObject(budgetJson);
            decimal monthlyLimit = budget.monthlyLimit;

            var totalThisMonth = await _context.Expenses
                .Where(e => e.UserId == dto.UserId &&
                            e.Date.Month == dto.Date.Month &&
                            e.Date.Year == dto.Date.Year)
                .SumAsync(e => e.Amount);

            // 2. If budget exceeded, send email via NotificationService
            if (totalThisMonth > monthlyLimit)
            {
                return Ok(new
                {
                    message = $"Expense saved . ⚠ You have exceeded your budget of ₹{monthlyLimit}.Total this month : ₹{totalThisMonth}"
                });
            }

            return Ok(new { message = "Expense saved successfully ✅" });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return Ok(expenses);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}