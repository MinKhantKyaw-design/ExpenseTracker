using ExpenseTracker.Common.Enums;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseListQueryHandler
{
    private readonly ExpenseTrackerDBContext _context;
    public GetExpenseListQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<IEnumerable<ExpenseDto>> HandleAsync(int userId, GetExpenseListRequest request)
    {
        var query = _context.Expenses
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.DelFlg);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.Date >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.Date <= request.ToDate.Value);

        if (request.CategoryID.HasValue)
            query = query.Where(x => x.CategoryId == request.CategoryID.Value);

        query = (request.SortBy, request.SortDirection) switch
        {
            (eExpenseListSortBy.SortByAmount, eSortOrder.Ascending) => query.OrderBy(x => x.Amount),
            (eExpenseListSortBy.SortByAmount, _) => query.OrderByDescending(x => x.Amount),
            (_, eSortOrder.Ascending) => query.OrderBy(x => x.Date),
            _ => query.OrderByDescending(x => x.Date)
        };

        return await query.Select(x => new ExpenseDto
        {
            ExpenseID = x.ExpenseId,
            UserID = x.UserId,
            CategoryID = x.CategoryId,
            CategoryName = x.Category.Name,
            Amount = x.Amount,
            Date = x.Date,
            Notes = x.Notes ?? string.Empty
        }).ToListAsync();
    }
}
