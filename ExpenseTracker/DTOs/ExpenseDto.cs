namespace ExpenseTracker.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseID { get; set; }

        public int UserID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal Amount { get; set; }

        public DateOnly Date { get; set; }

        public string Notes { get; set; }
    }
}