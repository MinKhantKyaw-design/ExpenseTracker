using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses.Commands;

public sealed class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly ExpenseValidator _validator;

    public CreateExpenseCommandHandler(ExpenseTrackerDBContext context, ExpenseValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand command, CancellationToken cancellationToken)
    {
        int userId = command.UserId;
        ExpenseRequest request = command.Request;
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
        await _context.SaveChangesAsync(cancellationToken);

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
            .SingleAsync(cancellationToken);
    }
}
