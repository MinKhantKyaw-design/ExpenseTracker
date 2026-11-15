using ExpenseTracker.Common.Enums;
using ExpenseTracker.Database.Context;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses
{
    public sealed class ExpenseQueryHandler : IExpenseQueryHandler
    {
        private readonly ExpenseTrackerDBContext _context;

        public ExpenseQueryHandler(ExpenseTrackerDBContext context)
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
    }
}
