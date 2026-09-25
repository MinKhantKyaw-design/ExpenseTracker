namespace ExpenseTracker.DTOs
{
    public class CategorySummaryDto
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
