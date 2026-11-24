using ExpenseTracker.DTOs.Requests;
using MediatR;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class UpdateExpenseCommand : IRequest<bool>
{
    public int ExpenseId { get; }
    public int UserId { get; }
    public ExpenseRequest Request { get; }

    public UpdateExpenseCommand(int expenseId, int userId, ExpenseRequest request)
    {
        ExpenseId = expenseId;
        UserId = userId;
        Request = request;
    }
}