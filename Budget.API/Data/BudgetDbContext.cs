using Microsoft.EntityFrameworkCore;
using Budget.API.Models;

namespace Budget.API.Data
{
    public class BudgetDbContext : DbContext
    {
        public BudgetDbContext(DbContextOptions options) : base(options) { }

        public  DbSet<UserBudget> Budgets { get; set; }
    }
}
