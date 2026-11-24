using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Features.Categories.Commands;
using ExpenseTracker.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[Authorize]
public class CategoryController : BaseController
{

    private readonly ISender _sender;

    public CategoryController(
        ISender sender)
    {
        _sender = sender;
    }

    public async Task<IActionResult> Index() => View(await _sender.Send(new GetCategoryListQuery()));

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _sender.Send(new CreateCategoryCommand(request));

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
        var category = await _sender.Send(new GetCategoryByIdQuery(id));
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
            bool updated = await _sender.Send(new UpdateCategoryCommand(id, request));
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
        var category = await _sender.Send(new GetCategoryByIdQuery(id));
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            bool deleted = await _sender.Send(new DeleteCategoryCommand(id));
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
