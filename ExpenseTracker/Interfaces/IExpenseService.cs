using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;

namespace ExpenseTracker.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetAllAsync(GetExpenseListRequest request);

        Task<ExpenseDto> GetByIdAsync(int id);

        Task<ExpenseDto> CreateAsync(ExpenseRequest request);

        Task<bool> UpdateAsync(int id, ExpenseRequest request);

        Task<bool> DeleteAsync(int id);
    }
}