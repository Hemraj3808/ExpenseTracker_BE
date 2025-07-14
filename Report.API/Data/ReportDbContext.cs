using Microsoft.EntityFrameworkCore;
using Report.API.Models;

namespace Report.API.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext (DbContextOptions<ReportDbContext> options) : base(options) { }
        public  DbSet<ExpenseReport> ExpenseReports { get; set; }
    }
}
