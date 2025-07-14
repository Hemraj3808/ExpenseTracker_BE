namespace Expense.Domain.Entities
{
    public class ExpenseItem
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
