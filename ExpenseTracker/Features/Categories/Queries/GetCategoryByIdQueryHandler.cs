using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Categories.Queries;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetCategoryByIdQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public Task<CategoryDto?> Handle(GetCategoryByIdQuery command, CancellationToken cancellationToken)
    {
        return _context.Categories.AsNoTracking()
            .Where(x => x.CategoryId == command.Id && !x.DelFlg)
            .Select(x => new CategoryDto
            {
                CategoryID = x.CategoryId,
                Name = x.Name,
                Description = x.Description ?? string.Empty
            }).FirstOrDefaultAsync();
    }

    public Task<CategoryDto?> HandleAsync(int id)
    {
        return _context.Categories.AsNoTracking()
            .Where(x => x.CategoryId == id && !x.DelFlg)
            .Select(x => new CategoryDto
            {
                CategoryID = x.CategoryId,
                Name = x.Name,
                Description = x.Description ?? string.Empty
            }).FirstOrDefaultAsync();
    }
}
