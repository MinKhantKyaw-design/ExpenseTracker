using ExpenseTracker.Common.Constants;
using ExpenseTracker.Database.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Features.Categories.Commands;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand,bool>
{
    private readonly ExpenseTrackerDBContext _context;
    public DeleteCategoryCommandHandler(ExpenseTrackerDBContext context) => _context = context;

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == command.Id && !x.DelFlg);
        if (category == null) return false;

        if (category.Name == "Miscellaneous")
            throw new InvalidOperationException(CategoryDataMessage.MiscellaneousCategoryDeleteError);

        bool hasExpenses = await _context.Expenses
            .AnyAsync(x => x.CategoryId == command.Id && !x.DelFlg);
        if (hasExpenses)
            throw new InvalidOperationException(CategoryDataMessage.DeleteHasExpenseError);

        category.DelFlg = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HandleAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == id && !x.DelFlg);
        if (category == null) return false;

        if (category.Name == "Miscellaneous")
            throw new InvalidOperationException(CategoryDataMessage.MiscellaneousCategoryDeleteError);

        bool hasExpenses = await _context.Expenses
            .AnyAsync(x => x.CategoryId == id && !x.DelFlg);
        if (hasExpenses)
            throw new InvalidOperationException(CategoryDataMessage.DeleteHasExpenseError);

        category.DelFlg = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
