using Azure.Core;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class UpdateCategoryCommandHandler : ICommandHandler<(int id, CategoryRequest request), bool>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly CategoryValidator _validator;

    public UpdateCategoryCommandHandler(ExpenseTrackerDBContext context, CategoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<bool> HandleAsync((int id, CategoryRequest request) command)
    {
        await _validator.ValidateCategoryInfoInput(command.request, command.id);
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == command.id && !x.DelFlg);
        if (category == null) return false;

        category.Name = command.request.Name.Trim();
        category.Description = command.request.Description?.Trim();
        await _context.SaveChangesAsync();
        await _context.SaveChangesAsync();
        return true;
    }
}
