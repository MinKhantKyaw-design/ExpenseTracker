using Azure.Core;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using MediatR;

namespace ExpenseTracker.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public CategoryRequest Request { get; set; }

        public UpdateCategoryCommand(int id, CategoryRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
