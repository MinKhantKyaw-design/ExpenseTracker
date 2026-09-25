using ExpenseTracker.DTOs;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;

        public ExpensesController(
            IExpenseService expenseService,
            ICategoryService categoryService)
        {
            _expenseService = expenseService;
            _categoryService = categoryService;
        }

        // GET: /Expenses
        public async Task<IActionResult> Index(
            DateOnly? fromDate,
            DateOnly? toDate,
            int? categoryId,
            string sortBy = "date",
            string sortDirection = "desc")
        {
            int userId = 1;

            var expenses = await _expenseService.GetAllAsync(
                userId,
                fromDate,
                toDate,
                categoryId,
                sortBy,
                sortDirection);

            ViewBag.Categories =
                await _categoryService.GetAllAsync();

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;

            return View(expenses);
        }

        // GET: /Expenses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories =
                await _categoryService.GetAllAsync();

            return View();
        }

        // POST: /Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ExpenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories =
                    await _categoryService.GetAllAsync();

                return View(request);
            }

            try
            {
                request.UserID = 1;

                await _expenseService.CreateAsync(request);

                TempData["SuccessMessage"] =
                    "Expense created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                ViewBag.Categories =
                    await _categoryService.GetAllAsync();

                return View(request);
            }
        }

        // GET: /Expenses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var expense =
                await _expenseService.GetByIdAsync(id);

            if (expense == null)
                return NotFound();

            var request = new ExpenseRequest
            {
                UserID = expense.UserID,
                CategoryID = expense.CategoryID,
                Amount = expense.Amount,
                Date = expense.Date,
                Notes = expense.Notes
            };

            ViewBag.ExpenseId = expense.ExpenseID;

            ViewBag.Categories =
                await _categoryService.GetAllAsync();

            return View(request);
        }

        // POST: /Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ExpenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExpenseId = id;

                ViewBag.Categories =
                    await _categoryService.GetAllAsync();

                return View(request);
            }

            try
            {

                var updated =
                    await _expenseService.UpdateAsync(id,request);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Expense updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                ViewBag.ExpenseId = id;

                ViewBag.Categories =
                    await _categoryService.GetAllAsync();

                return View(request);
            }
        }

        // GET: /Expenses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var expense =
                await _expenseService.GetByIdAsync(id);

            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // POST: /Expenses/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            try
            {
                var deleted =
                    await _expenseService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Expense deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
    }
}