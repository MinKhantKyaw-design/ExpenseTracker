using ExpenseTracker.DTOs;

namespace ExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodayTotal { get; set; }

        public decimal WeekTotal { get; set; }

        public decimal MonthTotal { get; set; }

        public IEnumerable<CategorySummaryDto> CategorySummary
        { get; set; }
    }
}