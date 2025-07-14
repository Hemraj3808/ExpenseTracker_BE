namespace Budget.API.Models
{
    public class UserBudget
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public int Month {  get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
