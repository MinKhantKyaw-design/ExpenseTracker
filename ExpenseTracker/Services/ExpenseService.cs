using ExpenseTracker.Common.Constants;
using ExpenseTracker.Common.Enums;
using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
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
        public async Task<IEnumerable<ExpenseDto>> GetAllAsync(GetExpenseListRequest request)
        {
            var query = _context.Expenses
                .Where(x => x.UserId == request.UserID && x.DelFlg == false);

            // Filter by From Date
            if (request.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date >= request.FromDate.Value);
            }

            // Filter by To Date
            if (request.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date <= request.ToDate.Value);
            }

            // Filter by Category
            if (request.CategoryID.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == request.CategoryID.Value);
            }

            // Sort
            if (request.SortBy == eExpenseListSortBy.SortByAmount)
            {
                if (request.SortDirection == eSortOrder.Ascending)
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
                if (request.SortDirection == eSortOrder.Ascending)
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
            var expense = new Expense
            {
                UserId = request.UserID,
                CategoryId = request.CategoryID ?? 0,
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

            expense.UserId = request.UserID;
            expense.CategoryId = request.CategoryID ?? 0;
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
    }
}