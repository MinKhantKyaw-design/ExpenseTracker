using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Dashboard.Queries;

public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardViewModel>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetDashboardQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<DashboardViewModel> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        int userId = query.UserId;
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        DateOnly startOfWeek = today.AddDays(-daysFromMonday);
        DateOnly startOfMonth = new(today.Year, today.Month, 1);

        var activeExpenses = _context.Expenses.AsNoTracking()
            .Where(x => x.UserId == userId && !x.DelFlg);

        var categorySummary = await activeExpenses
            .GroupBy(x => new { x.CategoryId, x.Category.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryID = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(cancellationToken);

        return new DashboardViewModel
        {
            TodayTotal = await activeExpenses
                .Where(x => x.Date == today)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            WeekTotal = await activeExpenses
                .Where(x => x.Date >= startOfWeek && x.Date < startOfWeek.AddDays(7))
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            MonthTotal = await activeExpenses
                .Where(x => x.Date >= startOfMonth && x.Date < startOfMonth.AddMonths(1))
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            CategorySummary = categorySummary
        };
    }
}
