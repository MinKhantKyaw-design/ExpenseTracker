using ExpenseTracker.DTOs;
using MediatR;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseByIdQuery : IRequest<ExpenseDto?>
{
    public int ExpenseId { get; }
    public int UserId { get; }

    public GetExpenseByIdQuery(int expenseId, int userId)
    {
        ExpenseId = expenseId;
        UserId = userId;
    }
}