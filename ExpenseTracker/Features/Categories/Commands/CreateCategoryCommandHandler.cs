using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class CreateCategoryCommandHandler
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly CategoryValidator _validator;

    public CreateCategoryCommandHandler(ExpenseTrackerDBContext context, CategoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<CategoryDto> HandleAsync(CategoryRequest request)
    {
        await _validator.ValidateCategoryInfoInput(request);
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DelFlg = false
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            CategoryID = category.CategoryId,
            Name = category.Name,
            Description = category.Description ?? string.Empty
        };
    }
}
