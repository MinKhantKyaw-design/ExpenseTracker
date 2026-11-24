using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly CategoryValidator _validator;

    public UpdateCategoryCommandHandler(ExpenseTrackerDBContext context, CategoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateCategoryInfoInput(command.Request, command.Id);
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == command.Id && !x.DelFlg);
        if (category == null) return false;

        category.Name = command.Request.Name.Trim();
        category.Description = command.Request.Description?.Trim();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HandleAsync(int id, CategoryRequest request)
    {
        await _validator.ValidateCategoryInfoInput(request, id);
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == id && !x.DelFlg);
        if (category == null) return false;

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        await _context.SaveChangesAsync();
        return true;
    }
}
