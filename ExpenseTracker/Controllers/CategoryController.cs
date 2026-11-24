using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Features.Categories.Commands;
using ExpenseTracker.Features.Categories.Queries;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[Authorize]
public class CategoryController : BaseController
{
    private readonly IQueryListHandler<IEnumerable<CategoryDto>> _getCategoryList;

    private readonly IQueryHandler<int, CategoryDto?> _getCategoryById;
    private readonly ICommandHandler<CategoryRequest, CategoryDto> _createCategory;
    private readonly ICommandHandler<(int, CategoryRequest), bool> _updateCategory;
    private readonly ICommandHandler<int, bool> _deleteCategory;

    public CategoryController(
        IQueryListHandler<IEnumerable<CategoryDto>> getList,
        IQueryHandler<int, CategoryDto?> getById,
        ICommandHandler<CategoryRequest, CategoryDto> create,
        ICommandHandler<(int, CategoryRequest), bool> update,
        ICommandHandler<int, bool> delete)
    {
        _getCategoryList = getList;
        _getCategoryById = getById;
        _createCategory = create;
        _updateCategory = update;
        _deleteCategory = delete;
    }

    public async Task<IActionResult> Index() => View(await _getCategoryList.HandleListAsync());

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _createCategory.HandleAsync(request);
            TempData["SuccessMessage"] = CategoryDataMessage.CategoryCreatedSuccess;
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            return View(request);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _getCategoryById.HandleAsync(id);
        if (category == null) return NotFound();
        ViewBag.CategoryId = category.CategoryID;
        return View(new CategoryRequest { Name = category.Name, Description = category.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CategoryId = id;
            return View(request);
        }
        try
        {
            bool updated = await _updateCategory.HandleAsync((id, request));
            if (!updated) return NotFound();
            TempData["SuccessMessage"] = CategoryDataMessage.CategoryEditedSuccess;
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            ViewBag.CategoryId = id;
            return View(request);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            ViewBag.CategoryId = id;
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _getCategoryById.HandleAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            bool deleted = await _deleteCategory.HandleAsync(id);
            if (!deleted) return NotFound();
            TempData["SuccessMessage"] = CategoryDataMessage.CategoryDeletedSuccess;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
