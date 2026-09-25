using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories =
                await _categoryService.GetAllAsync();

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
        public async Task<IActionResult> Create(
            CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                await _categoryService.CreateAsync(request);

                TempData["SuccessMessage"] =
                    "Category created successfully.";

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
                await _categoryService.GetByIdAsync(id);

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
        public async Task<IActionResult> Edit(
            int id, CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(request);
            }

            try
            {
                var updated =
                    await _categoryService.UpdateAsync(
                        id,
                        request);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Category updated successfully.";

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
                await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            try
            {
                var deleted =
                    await _categoryService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Category deleted successfully.";

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