using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;

namespace ExpenseTracker.Interfaces.Categories
{
    public interface ICategoryCommandHandler
    {
        Task<CategoryDto> CreateAsync(CategoryRequest request);

        Task<bool> UpdateAsync(int id, CategoryRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
