using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Expenses
{
    public sealed class ExpenseCommandHandler : IExpenseCommandHandler
    {

        private readonly ExpenseTrackerDBContext _context;

        private readonly ExpenseQueryHandler _expenseQueryHandler;

        public ExpenseCommandHandler(ExpenseTrackerDBContext context)
        {
            _context = context;
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

            return (await _context.Expenses
                .Where(x =>
                    x.ExpenseId == expense.ExpenseId &&
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
                .SingleAsync())!;
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
    }
}
