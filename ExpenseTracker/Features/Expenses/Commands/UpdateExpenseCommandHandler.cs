using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, bool>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly ExpenseValidator _validator;

    public UpdateExpenseCommandHandler(ExpenseTrackerDBContext context, ExpenseValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken cancellationToken)
    {
        int expenseId = command.ExpenseId;
        int userId = command.UserId;
        ExpenseRequest request = command.Request;
        // Never trust UserID posted by the browser.
        request.UserID = userId;
        await _validator.ValidateInputAndUserExist(request);
        request.CategoryID = await _validator.ValidateCategoryID(request.CategoryID);

        if (request.CategoryID is null or 0)
            throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);

        var expense = await _context.Expenses.FirstOrDefaultAsync(x =>
            x.ExpenseId == expenseId && x.UserId == userId && !x.DelFlg, cancellationToken);

        if (expense == null)
            return false;

        expense.CategoryId = request.CategoryID.Value;
        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.Notes = request.Notes?.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
