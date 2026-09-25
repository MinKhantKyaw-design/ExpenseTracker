using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
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
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Category name is required.");

            string categoryName = request.Name!.Trim(); // use ! after validation

            bool exists = await _context.Categories
                .AnyAsync(x => x.Name.ToLower() == categoryName.ToLower() && x.DelFlg == false);

            if (exists)
                throw new InvalidOperationException(
                    "A category with this name already exists.");

            var category = new Category
            {
                Name = categoryName,
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

        public async Task<bool> UpdateAsync(
            int id,
            CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Category name is required.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.DelFlg == false);

            if (category == null)
                return false;

            string categoryName = request.Name!.Trim(); // use ! after validation

            bool exists = await _context.Categories
                .AnyAsync(x =>
                    x.CategoryId != id &&
                    x.Name.ToLower() == categoryName.ToLower() &&
                    x.DelFlg == false);

            if (exists)
                throw new InvalidOperationException(
                    "A category with this name already exists.");

            category.Name = categoryName;
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
                    "The Miscellaneous category cannot be deleted.");
            }

            bool hasExpenses = await _context.Expenses
                .AnyAsync(x =>
                    x.CategoryId == id &&
                    x.DelFlg == false);

            if (hasExpenses)
            {
                throw new InvalidOperationException(
                    "Cannot delete a category that has expenses.");
            }

            // Soft delete
            category.DelFlg = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}