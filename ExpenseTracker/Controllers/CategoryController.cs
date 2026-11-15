using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces.Categories;
using ExpenseTracker.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly CategoryValidator _categoryValidator;
        private readonly ICategoryCommandHandler _categoryCommandHandler;

        private readonly ICategoryQueryHandler _categoryQueryHandler;

        public CategoryController(
            CategoryValidator categoryValidator,
            ICategoryCommandHandler categoryCommandHandler,
            ICategoryQueryHandler categoryQueryHandler)

        {
            _categoryValidator = categoryValidator;
            _categoryCommandHandler = categoryCommandHandler;
            _categoryQueryHandler = categoryQueryHandler;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories =
                await _categoryQueryHandler.GetAllAsync();

            return View(categories);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                await _categoryValidator.ValidateCategoryInfoInput(request);

                await _categoryCommandHandler.CreateAsync(request);

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

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category =
                await _categoryQueryHandler.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            var request = new CategoryRequest
            {
                Name = category.Name,
                Description = category.Description
            };

            ViewBag.CategoryId = category.CategoryID;

            return View(request);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( int id, CategoryRequest request )
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(request);
            }

            try
            {
                await _categoryValidator.ValidateCategoryInfoInput(request, id);

                var updated = await _categoryCommandHandler.UpdateAsync( id, request);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] = CategoryDataMessage.CategoryEditedSuccess;

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    "Name",
                    ex.Message);

                ViewBag.CategoryId = id;

                return View(request);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(request);
            }
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category =
                await _categoryQueryHandler.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var deleted =
                    await _categoryCommandHandler.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["SuccessMessage"] = CategoryDataMessage.CategoryDeletedSuccess;

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
    }
}