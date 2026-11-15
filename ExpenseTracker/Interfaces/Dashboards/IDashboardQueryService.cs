using ExpenseTracker.DTOs;

namespace ExpenseTracker.Interfaces.Dashboards
{
    public interface IDashboardQueryService
    {
        Task<decimal> GetTodayTotalAsync(int userId);

        Task<decimal> GetThisWeekTotalAsync(int userId);

        Task<decimal> GetThisMonthTotalAsync(int userId);

        Task<IEnumerable<CategorySummaryDto>> GetCategorySummaryAsync(
            int userId);
    }
}
