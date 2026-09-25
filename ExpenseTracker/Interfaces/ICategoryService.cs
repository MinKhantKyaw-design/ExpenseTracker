using ExpenseTracker.DTOs;

namespace ExpenseTracker.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();

        Task<CategoryDto> GetByIdAsync(int id);

        Task<CategoryDto> CreateAsync(CategoryRequest request);

        Task<bool> UpdateAsync(int id, CategoryRequest request);

        Task<bool> DeleteAsync(int id);
    }
}