using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using MediatR;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class CreateCategoryCommand : IRequest<CategoryDto>
{
    public CategoryRequest Request { get; set; }

    public CreateCategoryCommand(CategoryRequest request)
    {
        Request = request;
    }
}