using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseByIdQueryHandler : IQueryHandler<(int expenseId, int userId), ExpenseDto?>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetExpenseByIdQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public Task<ExpenseDto?> HandleAsync((int expenseId, int userId) command)
    {
        return _context.Expenses.AsNoTracking()
            .Where(x => x.ExpenseId == command.expenseId && x.UserId == command.userId && !x.DelFlg)
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
