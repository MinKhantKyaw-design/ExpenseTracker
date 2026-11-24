using ExpenseTracker.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class DeleteExpenseCommandHandler
{
    private readonly ExpenseTrackerDBContext _context;

    public DeleteExpenseCommandHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<bool> HandleAsync(int expenseId, int userId)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(x =>
            x.ExpenseId == expenseId && x.UserId == userId && !x.DelFlg);

        if (expense == null)
            return false;

        expense.DelFlg = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
