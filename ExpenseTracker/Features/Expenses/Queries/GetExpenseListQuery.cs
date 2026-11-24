using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using MediatR;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseListQuery : IRequest<IEnumerable<ExpenseDto>>
{
    public int UserId { get; }
    public GetExpenseListRequest Request { get; }

    public GetExpenseListQuery(int userId, GetExpenseListRequest request)
    {
        UserId = userId;
        Request = request;
    }
}