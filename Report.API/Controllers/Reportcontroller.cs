using Microsoft.AspNetCore.Mvc;
using Report.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Report.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reportcontroller : ControllerBase
    {
      private readonly ReportDbContext _context;

        public Reportcontroller(ReportDbContext context)
        {
            _context = context;
        }
        [HttpGet("user/{userId}/category-summary")]
        public async Task<IActionResult> GetCategorySummary(int userId)
        {
            var sumary = await _context.ExpenseReports
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    category = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync();
            return Ok(sumary);
        }

        [HttpGet("user/{userId}/monthly-summary")]
        public async Task<IActionResult> GetMonthlySummary(int userId)
        {
            var sumary = await _context.ExpenseReports
                .Where(e => e.UserId == userId)
                .GroupBy(e => new {e.ExpenseDate.Year,e.ExpenseDate.Month})
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            return Ok(sumary);
        }
    }
}
