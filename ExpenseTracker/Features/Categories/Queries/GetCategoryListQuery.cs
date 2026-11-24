using ExpenseTracker.DTOs;
using MediatR;

namespace ExpenseTracker.Features.Categories.Queries
{
    public class GetCategoryListQuery : IRequest<IEnumerable<CategoryDto>>
    {
    }
}
