using ExpenseTracker.Database.Context;
using ExpenseTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class DeleteExpenseCommandHandler : ICommandHandler<(int expenseId, int userId), bool>
{
    private readonly ExpenseTrackerDBContext _context;

    public DeleteExpenseCommandHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<bool> HandleAsync((int expenseId, int userId) command)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(x =>
            x.ExpenseId == command.expenseId && x.UserId == command.userId && !x.DelFlg);

        if (expense == null)
            return false;

        expense.DelFlg = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
