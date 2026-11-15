using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;

namespace ExpenseTracker.Interfaces.Expenses
{
    public interface IExpenseQueryHandler
    {
        Task<IEnumerable<ExpenseDto>> GetAllAsync(GetExpenseListRequest request);

        Task<ExpenseDto> GetByIdAsync(int id);
    }
}
