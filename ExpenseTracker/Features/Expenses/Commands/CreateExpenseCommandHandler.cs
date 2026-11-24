using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Validators;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class CreateExpenseCommandHandler : ICommandHandler<(int userId, ExpenseRequest request), ExpenseDto>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly ExpenseValidator _validator;

    public CreateExpenseCommandHandler(ExpenseTrackerDBContext context, ExpenseValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<ExpenseDto> HandleAsync((int userId, ExpenseRequest request) command)
    {
        command.request.UserID = command.userId;
        await _validator.ValidateInputAndUserExist(command.request);
        command.request.CategoryID = await _validator.ValidateCategoryID(command.request.CategoryID);

        if (command.request.CategoryID is null or 0)
            throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);

        var expense = new Expense
        {
            UserId = command.userId,
            CategoryId = command.request.CategoryID.Value,
            Amount = command.request.Amount,
            Date = command.request.Date,
            Notes = command.request.Notes?.Trim(),
            DelFlg = false
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return await _context.Expenses
            .Where(x => x.ExpenseId == expense.ExpenseId && x.UserId == command.userId && !x.DelFlg)
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
            .SingleAsync();
    }
}
