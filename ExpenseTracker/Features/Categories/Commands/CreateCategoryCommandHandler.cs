using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Validators;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ExpenseTrackerDBContext _context;
    private readonly CategoryValidator _validator;

    public CreateCategoryCommandHandler(ExpenseTrackerDBContext context, CategoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {

        await _validator.ValidateCategoryInfoInput(command.Request);

        var category = new Category
        {
            Name = command.Request.Name.Trim(),
            Description = command.Request.Description?.Trim(),
            DelFlg = false
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            CategoryID = category.CategoryId,
            Name = category.Name,
            Description = category.Description ?? string.Empty
        };
    }
}
