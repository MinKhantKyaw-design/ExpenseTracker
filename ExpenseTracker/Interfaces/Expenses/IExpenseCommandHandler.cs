using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;

namespace ExpenseTracker.Interfaces.Expenses
{
    public interface IExpenseCommandHandler
    {
        Task<ExpenseDto> CreateAsync(ExpenseRequest request);

        Task<bool> UpdateAsync(int id, ExpenseRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
