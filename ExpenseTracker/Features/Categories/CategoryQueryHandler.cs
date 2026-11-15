using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces.Categories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Categories
{
    public sealed class CategoryQueryHandler : ICategoryQueryHandler
    {

        private readonly ExpenseTrackerDBContext _context;

        public CategoryQueryHandler(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Where(x => x.DelFlg == false)
                .Select(x => new CategoryDto
                {
                    CategoryID = x.CategoryId,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id) // changed to nullable
        {
            return await _context.Categories
                .Where(x => x.CategoryId == id && x.DelFlg == false)
                .Select(x => new CategoryDto
                {
                    CategoryID = x.CategoryId,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty
                })
                .FirstOrDefaultAsync();
        }
    }
}
