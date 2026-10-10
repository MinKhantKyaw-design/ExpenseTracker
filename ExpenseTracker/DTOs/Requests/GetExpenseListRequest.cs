using ExpenseTracker.Common.Enums;

namespace ExpenseTracker.DTOs.Requests
{
    public class GetExpenseListRequest
    {
        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }

        public int UserID { get; set; } = 0; // Implement for later

        public int? CategoryID { get; set; }

        public eExpenseListSortBy SortBy { get; set; } = eExpenseListSortBy.SortByDate;

        public eSortOrder SortDirection { get; set; } = eSortOrder.Descending;
    }
}
