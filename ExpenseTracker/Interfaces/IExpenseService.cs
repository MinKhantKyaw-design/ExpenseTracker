using ExpenseTracker.DTOs;

namespace ExpenseTracker.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetAllAsync(int userId, DateOnly? fromDate = null, DateOnly? toDate = null, int? categoryId = null, string sortBy = "date", string sortDirection = "desc");

        Task<ExpenseDto> GetByIdAsync(int id);

        Task<ExpenseDto> CreateAsync(ExpenseRequest request);

        Task<bool> UpdateAsync(int id, ExpenseRequest request);

        Task<bool> DeleteAsync(int id);
    }
}