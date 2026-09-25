using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseTrackerDBContext _context;

        public ExpenseService(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        // Get expense list
        // Supports:
        // - Date range filter
        // - Category filter
        // - Sort by date
        // - Sort by amount
        public async Task<IEnumerable<ExpenseDto>> GetAllAsync(
            int userId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            int? categoryId = null,
            string sortBy = "date",
            string sortDirection = "desc")
        {
            var query = _context.Expenses
                .Where(x =>
                    x.UserId == userId &&
                    x.DelFlg == false);

            // Filter by From Date
            if (fromDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date >= fromDate.Value);
            }

            // Filter by To Date
            if (toDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date <= toDate.Value);
            }

            // Filter by Category
            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            // Sort
            if (sortBy?.ToLower() == "amount")
            {
                if (sortDirection?.ToLower() == "asc")
                {
                    query = query.OrderBy(x => x.Amount);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Amount);
                }
            }
            else
            {
                if (sortDirection?.ToLower() == "asc")
                {
                    query = query.OrderBy(x => x.Date);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Date);
                }
            }

            return await query
                .Select(x => new ExpenseDto
                {
                    ExpenseID = x.ExpenseId,
                    UserID = x.UserId,
                    CategoryID = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Amount = x.Amount,
                    Date = x.Date,
                    Notes = x.Notes ?? string.Empty
                })
                .ToListAsync();
        }


        // Get expense by ID
        public async Task<ExpenseDto?> GetByIdAsync(int id)
        {
            return await _context.Expenses
                .Where(x =>
                    x.ExpenseId == id &&
                    x.DelFlg == false)
                .Select(x => new ExpenseDto
                {
                    ExpenseID = x.ExpenseId,
                    UserID = x.UserId,
                    CategoryID = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Amount = x.Amount,
                    Date = x.Date,
                    Notes = x.Notes ?? string.Empty
                })
                .FirstOrDefaultAsync();
        }


        // Create expense
        public async Task<ExpenseDto> CreateAsync(
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
                throw new ArgumentException(
                    "User does not exist.");
            }

            int categoryId;

            // Category selected
            if (request.CategoryID.HasValue &&
                request.CategoryID.Value > 0)
            {
                categoryId = request.CategoryID.Value;

                var categoryExists =
                    await _context.Categories
                        .AnyAsync(x =>
                            x.CategoryId == categoryId &&
                            x.DelFlg == false);

                if (!categoryExists)
                {
                    throw new ArgumentException(
                        "Category does not exist.");
                }
            }
            // No category selected
            // Use Miscellaneous
            else
            {
                categoryId = await _context.Categories
                    .Where(x =>
                        x.Name == "Miscellaneous" &&
                        x.DelFlg == false)
                    .Select(x => x.CategoryId)
                    .SingleOrDefaultAsync();

                if (categoryId == 0)
                {
                    throw new InvalidOperationException(
                        "Miscellaneous category does not exist.");
                }
            }

            var expense = new Expense
            {
                UserId = request.UserID,
                CategoryId = categoryId,
                Amount = request.Amount,
                Date = request.Date,
                Notes = request.Notes?.Trim(),
                DelFlg = false
            };

            _context.Expenses.Add(expense);

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(expense.ExpenseId))!;
        }


        // Update expense
        public async Task<bool> UpdateAsync(
            int id,
            ExpenseRequest request)
        {
            ValidateExpense(request);

            // Find expense belonging to current user
            // and not soft-deleted
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x =>
                    x.ExpenseId == id &&
                    x.DelFlg == false);

            if (expense == null)
            {
                return false;
            }

            // Validate User
            var userExists = await _context.Users
                .AnyAsync(x =>
                    x.UserId == request.UserID &&
                    x.DelFlg == false);

            if (!userExists)
            {
                throw new ArgumentException(
                    "User does not exist.");
            }

            int categoryId;

            // Category selected
            if (request.CategoryID.HasValue &&
                request.CategoryID.Value > 0)
            {
                categoryId = request.CategoryID.Value;

                var categoryExists =
                    await _context.Categories
                        .AnyAsync(x =>
                            x.CategoryId == categoryId &&
                            x.DelFlg == false);

                if (!categoryExists)
                {
                    throw new ArgumentException(
                        "Category does not exist.");
                }
            }
            // No category selected
            // Use Miscellaneous
            else
            {
                categoryId = await _context.Categories
                    .Where(x =>
                        x.Name == "Miscellaneous" &&
                        x.DelFlg == false)
                    .Select(x => x.CategoryId)
                    .SingleOrDefaultAsync();

                if (categoryId == 0)
                {
                    throw new InvalidOperationException(
                        "Miscellaneous category does not exist.");
                }
            }

            expense.UserId = request.UserID;
            expense.CategoryId = categoryId;
            expense.Amount = request.Amount;
            expense.Date = request.Date;
            expense.Notes = request.Notes?.Trim();

            await _context.SaveChangesAsync();

            return true;
        }


        // Soft delete expense
        public async Task<bool> DeleteAsync(int id)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x =>
                    x.ExpenseId == id &&
                    x.DelFlg == false);

            if (expense == null)
            {
                return false;
            }

            // Soft delete
            expense.DelFlg = true;

            await _context.SaveChangesAsync();

            return true;
        }


        // Validate expense data
        private static void ValidateExpense(
            ExpenseRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Expense amount must be greater than 0.");
            }

            if (request.Date == default)
            {
                throw new ArgumentException(
                    "Expense date is required.");
            }

            if (request.Date >
                DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException(
                    "Expense date cannot be in the future.");
            }
        }
    }
}