namespace ExpenseTracker.DTOs.Requests
{
    public class ExpenseRequest
    {
        public int UserID { get; set; }

        public int? CategoryID { get; set; }

        public decimal Amount { get; set; }

        public DateOnly Date { get; set; }

        public string Notes { get; set; }
    }
}