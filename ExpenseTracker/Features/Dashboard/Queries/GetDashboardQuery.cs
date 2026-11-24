using ExpenseTracker.ViewModels;
using MediatR;

namespace ExpenseTracker.Features.Dashboard.Queries;

public sealed class GetDashboardQuery : IRequest<DashboardViewModel>
{
    public int UserId { get; }

    public GetDashboardQuery(int userId)
    {
        UserId = userId;
    }
}