using ExpenseTracker.Common;
using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Validators
{
    public class CategoryValidator
    {
        private readonly ExpenseTrackerDBContext _context;

        public CategoryValidator(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        public async Task ValidateCategoryInfoInput( CategoryRequest request, int id = 0)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException(
                    CategoryDataMessage.CategoryNameRequiredError);
            }

            string categoryName = request.Name.Trim();

            bool exists = await _context.Categories.AnyAsync(x =>
                x.CategoryId != id &&
                x.Name.ToLower() == categoryName.ToLower() &&
                x.DelFlg == false);

            if (exists)
            {
                throw new InvalidOperationException(
                    CategoryDataMessage.CategoryNameExistError);
            }
        }
    }
}