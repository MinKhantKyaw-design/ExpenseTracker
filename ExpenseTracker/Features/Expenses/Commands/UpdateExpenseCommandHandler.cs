using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Validators;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class UpdateExpenseCommandHandler : ICommandHandler<(int expenseId, int userId, ExpenseRequest request), bool>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly ExpenseValidator _validator;

    public UpdateExpenseCommandHandler(ExpenseTrackerDBContext context, ExpenseValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<bool> HandleAsync((int expenseId, int userId, ExpenseRequest request) command)
    {
        // Never trust UserID posted by the browser.
        command.request.UserID = command.userId;
        await _validator.ValidateInputAndUserExist(command.request);
        command.request.CategoryID = await _validator.ValidateCategoryID(command.request.CategoryID);

        if (command.request.CategoryID is null or 0)
            throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);

        var expense = await _context.Expenses.FirstOrDefaultAsync(x =>
            x.ExpenseId == command.expenseId && x.UserId == command.userId && !x.DelFlg);

        if (expense == null)
            return false;

        expense.CategoryId = command.request.CategoryID.Value;
        expense.Amount = command.request.Amount;
        expense.Date = command.request.Date;
        expense.Notes = command.request.Notes?.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> HandleAsync(int command)
    {
        throw new NotImplementedException();
    }
}
