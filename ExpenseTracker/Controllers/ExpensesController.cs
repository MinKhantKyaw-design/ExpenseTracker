using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Features.Categories.Queries;
using ExpenseTracker.Features.Expenses.Commands;
using ExpenseTracker.Features.Expenses.Queries;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[Authorize]
public class ExpensesController : BaseController
{
    private readonly IQueryHandler<(int userId, GetExpenseListRequest request), IEnumerable<ExpenseDto>> _getList;
    private readonly IQueryHandler<(int expenseId, int userId), ExpenseDto?> _getById;
    private readonly ICommandHandler<(int, ExpenseRequest), ExpenseDto> _create;
    private readonly ICommandHandler<(int expenseId, int userId, ExpenseRequest request), bool> _update;
    private readonly ICommandHandler<(int expenseId, int userId), bool> _delete;
    private readonly IQueryListHandler<IEnumerable<CategoryDto>> _getCategoriesList;

    public ExpensesController(
        IQueryHandler<(int userId, GetExpenseListRequest request), IEnumerable<ExpenseDto>> getList,
        IQueryHandler<(int expenseId, int userId), ExpenseDto?> getById,
        ICommandHandler<(int, ExpenseRequest), ExpenseDto> create,
        ICommandHandler<(int expenseId, int userId, ExpenseRequest request), bool> update,
        ICommandHandler<(int expenseId, int userId), bool> delete,
        IQueryListHandler<IEnumerable<CategoryDto>> getCategories)
    {
        _getList = getList;
        _getById = getById;
        _create = create;
        _update = update;
        _delete = delete;
        _getCategoriesList = getCategories;
    }

    public async Task<IActionResult> Index(GetExpenseListRequest request)
    {
        int userId = GetCurrentUserId();
        var expenses = await _getList.HandleAsync((userId, request));
        ViewBag.Categories = await _getCategoriesList.HandleListAsync();
        ViewBag.FromDate = request.FromDate;
        ViewBag.ToDate = request.ToDate;
        ViewBag.CategoryId = request.CategoryID;
        ViewBag.SortBy = request.SortBy;
        ViewBag.SortDirection = request.SortDirection;
        return View(expenses);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _getCategoriesList.HandleListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _getCategoriesList.HandleListAsync();
            return View(request);
        }

        try
        {
            await _create.HandleAsync((GetCurrentUserId(), request));
            TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseCreatedSuccess;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Categories = await _getCategoriesList.HandleListAsync();
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _getById.HandleAsync((id, GetCurrentUserId()));
        if (expense == null) return NotFound();

        var request = new ExpenseRequest
        {
            UserID = expense.UserID,
            CategoryID = expense.CategoryID,
            Amount = expense.Amount,
            Date = expense.Date,
            Notes = expense.Notes
        };

        ViewBag.ExpenseId = expense.ExpenseID;
        ViewBag.Categories = await _getCategoriesList.HandleListAsync();
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int expenseID, ExpenseRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ExpenseId = expenseID;
            ViewBag.Categories = await _getCategoriesList.HandleListAsync();
            return View(request);
        }

        try
        {
            bool updated = await _update.HandleAsync((expenseID, GetCurrentUserId(), request));
            if (!updated) return NotFound();

            TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseEditedSuccess;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.ExpenseId = expenseID;
            ViewBag.Categories = await _getCategoriesList.HandleListAsync();
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _getById.HandleAsync((id, GetCurrentUserId()));
        if (expense == null) return NotFound();
        return View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await _delete.HandleAsync((id, GetCurrentUserId()));
        if (!deleted) return NotFound();

        TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseDeletedSuccess;
        return RedirectToAction(nameof(Index));
    }
}
