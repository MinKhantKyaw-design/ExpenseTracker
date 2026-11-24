using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Categories.Queries;

public sealed class GetCategoryListQueryHandler : IRequestHandler<GetCategoryListQuery, IEnumerable<CategoryDto>>
{
    private readonly ExpenseTrackerDBContext _context;
    public GetCategoryListQueryHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories.AsNoTracking()
           .Where(x => !x.DelFlg)
           .OrderBy(x => x.Name)
           .Select(x => new CategoryDto
           {
               CategoryID = x.CategoryId,
               Name = x.Name,
               Description = x.Description ?? string.Empty
            }).ToListAsync(cancellationToken);
    }

    //public async Task<IEnumerable<CategoryDto>> HandleAsync()
    //{
    //    return await _context.Categories.AsNoTracking()
    //        .Where(x => !x.DelFlg)
    //        .OrderBy(x => x.Name)
    //        .Select(x => new CategoryDto
    //        {
    //            CategoryID = x.CategoryId,
    //            Name = x.Name,
    //            Description = x.Description ?? string.Empty
    //        }).ToListAsync();
    //}
}
