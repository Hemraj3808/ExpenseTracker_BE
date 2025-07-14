using Budget.API.Data;
using Budget.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Budget.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly BudgetDbContext _context;

        public BudgetController(BudgetDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public async Task<IActionResult> SetBudget(UserBudget budget)
        {
            var existing = await _context.Budgets
                .FirstOrDefaultAsync(b =>
                    b.UserId == budget.UserId &&
                    b.Month == budget.Month &&
                    b.Year == budget.Year);

            if (existing != null)
            {
                existing.MonthlyLimit = budget.MonthlyLimit;
            }
            else
            {
                await _context.Budgets.AddAsync(budget);
            }

            await _context.SaveChangesAsync();
            return Ok(budget);
        }

       
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBudgets(int userId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(budgets);
        }

      
        [HttpGet("user/{userId}/current")]
        public async Task<IActionResult> GetCurrentMonthBudget(int userId)
        {
            var now = DateTime.UtcNow;
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.Month == now.Month &&
                    b.Year == now.Year);

            return budget != null ? Ok(budget) : NotFound("No budget set for current month.");
        }
    }
}
