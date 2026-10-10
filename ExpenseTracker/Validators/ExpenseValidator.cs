using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Validators
{
    public class ExpenseValidator
    {
        private readonly ExpenseTrackerDBContext _context;

        public ExpenseValidator(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        // Create expense
        public async Task ValidateInputAndUserExist(
            ExpenseRequest request)
        {
            ValidateExpense(request);

            // Validate User
            var userExists = await _context.Users
                .AnyAsync(x =>
                    x.UserId == request.UserID &&
                    x.DelFlg == false);

            if (!userExists)
            {
                throw new ArgumentException(ExpenseDataMessage.UserDoesNotExistError);
            }
        }

        // Create expense
        public async Task<int?> ValidateCategoryID(
            int? categoryID)
        {
            // Category selected
            if (categoryID.HasValue &&
                categoryID.Value > 0)
            {
                var categoryExists =
                    await _context.Categories
                        .AnyAsync(x =>
                            x.CategoryId == categoryID &&
                            x.DelFlg == false);

                if (!categoryExists)
                {
                    throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);
                }
            }
            // No category selected
            // Use Miscellaneous
            else
            {
                categoryID = await _context.Categories
                    .Where(x =>
                        x.Name == "Miscellaneous" &&
                        x.DelFlg == false)
                    .Select(x => x.CategoryId)
                    .SingleOrDefaultAsync();

                if (categoryID == 0)
                {
                    throw new InvalidOperationException(
                        ExpenseDataMessage.MiscellaneousCategoryDoesNotExistError);
                }
            }
            return categoryID;
        }

        #region Private Methods

        // Validate expense data
        private static void ValidateExpense(
            ExpenseRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    ExpenseDataMessage.ExpenseLessThanZeroError);
            }

            if (request.Date == default)
            {
                throw new ArgumentException(
                    ExpenseDataMessage.ExpenseDateRequireError);
            }

            if (request.Date >
                DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException(
                    ExpenseDataMessage.ExpenseDateInFutureError);
            }
        }

        #endregion
    }
}
