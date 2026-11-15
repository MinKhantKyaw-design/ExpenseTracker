using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseByIdQueryHandler
{
    private readonly ExpenseTrackerDBContext _context;
    public GetExpenseByIdQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public Task<ExpenseDto?> HandleAsync(int expenseId, int userId)
    {
        return _context.Expenses.AsNoTracking()
            .Where(x => x.ExpenseId == expenseId && x.UserId == userId && !x.DelFlg)
            .Select(x => new ExpenseDto
            {
                ExpenseID = x.ExpenseId,
                UserID = x.UserId,
                CategoryID = x.CategoryId,
                CategoryName = x.Category.Name,
                Amount = x.Amount,
                Date = x.Date,
                Notes = x.Notes ?? string.Empty
            })
            .FirstOrDefaultAsync();
    }
}
