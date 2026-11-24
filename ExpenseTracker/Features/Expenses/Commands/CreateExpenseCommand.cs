using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using MediatR;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class CreateExpenseCommand : IRequest<ExpenseDto>
{
    public int UserId { get; }
    public ExpenseRequest Request { get; }

    public CreateExpenseCommand(int userId, ExpenseRequest request)
    {
        UserId = userId;
        Request = request;
    }
}