using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class CreateExpenseCommandHandler
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly ExpenseValidator _validator;

    public CreateExpenseCommandHandler(ExpenseTrackerDBContext context, ExpenseValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<ExpenseDto> HandleAsync(int userId, ExpenseRequest request)
    {
        request.UserID = userId;
        await _validator.ValidateInputAndUserExist(request);
        request.CategoryID = await _validator.ValidateCategoryID(request.CategoryID);

        if (request.CategoryID is null or 0)
            throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);

        var expense = new Expense
        {
            UserId = userId,
            CategoryId = request.CategoryID.Value,
            Amount = request.Amount,
            Date = request.Date,
            Notes = request.Notes?.Trim(),
            DelFlg = false
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return await _context.Expenses
            .Where(x => x.ExpenseId == expense.ExpenseId && x.UserId == userId && !x.DelFlg)
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
