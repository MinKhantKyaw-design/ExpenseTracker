using ExpenseTracker.Common.Enums;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseListQueryHandler : IQueryHandler<(int userId, GetExpenseListRequest request),IEnumerable<ExpenseDto>>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetExpenseListQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<IEnumerable<ExpenseDto>> HandleAsync((int userId, GetExpenseListRequest request) command)
    {
        var query = _context.Expenses
            .AsNoTracking()
            .Where(x => x.UserId == command.userId && !x.DelFlg);

        if (command.request.FromDate.HasValue)
            query = query.Where(x => x.Date >= command.request.FromDate.Value);

        if (command.request.ToDate.HasValue)
            query = query.Where(x => x.Date <= command.request.ToDate.Value);

        if (command.request.CategoryID.HasValue)
            query = query.Where(x => x.CategoryId == command.request.CategoryID.Value);

        query = (command.request.SortBy, command.request.SortDirection) switch
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
