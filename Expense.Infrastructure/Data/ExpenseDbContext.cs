using Expense.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Expense.Infrastructure.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }
        public DbSet<ExpenseItem> Expenses => Set<ExpenseItem>();
    }
}
