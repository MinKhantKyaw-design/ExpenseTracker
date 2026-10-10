using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ExpenseTrackerDBContext _context;

        public CategoryService(ExpenseTrackerDBContext context)
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

        public async Task<CategoryDto> CreateAsync(CategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name!.Trim(),
                Description = request.Description?.Trim()
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

        public async Task<bool> UpdateAsync(int id, CategoryRequest request)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.DelFlg == false);

            if (category == null)
                return false;

            category.Name = request.Name!.Trim();
            category.Description = request.Description?.Trim();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x =>
                    x.CategoryId == id &&
                    x.DelFlg == false);

            if (category == null)
                return false;

            if (category.Name == "Miscellaneous")
            {
                throw new InvalidOperationException(
                    CategoryDataMessage.MiscellaneousCategoryDeleteError);
            }

            bool hasExpenses = await _context.Expenses
                .AnyAsync(x =>
                    x.CategoryId == id &&
                    x.DelFlg == false);

            if (hasExpenses)
            {
                throw new InvalidOperationException(
                    CategoryDataMessage.DeleteHasExpenseError);
            }

            // Soft delete
            category.DelFlg = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}