using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ExpenseTrackerDBContext _context;

        public DashboardService(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTodayTotalAsync(int userId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Expenses
                .Where(x =>
                    x.UserId == userId &&
                    x.Date == today)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<decimal> GetThisWeekTotalAsync(int userId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);


            // T.B.D : Add in method
            int difference =(7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-difference);
            var endOfWeek = startOfWeek.AddDays(7);

            return await _context.Expenses
                .Where(x =>
                    x.UserId == userId &&
                    x.Date >= startOfWeek &&
                    x.Date < endOfWeek)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<decimal> GetThisMonthTotalAsync(int userId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            var startOfMonth =
                new DateOnly(today.Year, today.Month, 1);

            var startOfNextMonth =
                startOfMonth.AddMonths(1);

            return await _context.Expenses
                .Where(x =>
                    x.UserId == userId &&
                    x.Date >= startOfMonth &&
                    x.Date < startOfNextMonth)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummaryAsync(int userId)
        {
            return await _context.Expenses
                .Where(x => x.UserId == userId)
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Category.Name
                })
                .Select(x => new CategorySummaryDto
                {
                    CategoryID = x.Key.CategoryId,
                    CategoryName = x.Key.Name,
                    TotalAmount = x.Sum(e => e.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();
        }
    }
}