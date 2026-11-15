using ExpenseTracker.DTOs;

namespace ExpenseTracker.Interfaces.Categories
{
    public interface ICategoryQueryHandler
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();

        Task<CategoryDto> GetByIdAsync(int id);
    }
}
