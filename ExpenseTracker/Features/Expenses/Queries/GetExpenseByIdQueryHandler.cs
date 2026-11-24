using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Queries;

public sealed class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto?>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetExpenseByIdQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public Task<ExpenseDto?> Handle(GetExpenseByIdQuery query, CancellationToken cancellationToken)
    {
        return _context.Expenses.AsNoTracking()
            .Where(x => x.ExpenseId == query.ExpenseId && x.UserId == query.UserId && !x.DelFlg)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
