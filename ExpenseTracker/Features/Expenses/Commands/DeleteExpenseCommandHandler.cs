using ExpenseTracker.Database.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, bool>
{
    private readonly ExpenseTrackerDBContext _context;

    public DeleteExpenseCommandHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken cancellationToken)
    {
        int expenseId = command.ExpenseId;
        int userId = command.UserId;
        var expense = await _context.Expenses.FirstOrDefaultAsync(x =>
            x.ExpenseId == expenseId && x.UserId == userId && !x.DelFlg, cancellationToken);

        if (expense == null)
            return false;

        expense.DelFlg = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
