using MediatR;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class DeleteExpenseCommand : IRequest<bool>
{
    public int ExpenseId { get; }
    public int UserId { get; }

    public DeleteExpenseCommand(int expenseId, int userId)
    {
        ExpenseId = expenseId;
        UserId = userId;
    }
}