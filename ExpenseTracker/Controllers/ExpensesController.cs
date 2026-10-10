using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Validators;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;
        private readonly ExpenseValidator _expenseValidator;

        public ExpensesController(
            IExpenseService expenseService,
            ICategoryService categoryService,
            ExpenseValidator expenseValidator)
        {
            _expenseService = expenseService;
            _categoryService = categoryService;
            _expenseValidator = expenseValidator;
        }

        // GET: /Expenses
        public async Task<IActionResult> Index(GetExpenseListRequest request)
        {
            request.UserID = 1;

            var expenses = await _expenseService.GetAllAsync(request);

            ViewBag.Categories = await _categoryService.GetAllAsync();

            ViewBag.FromDate = request.FromDate;
            ViewBag.ToDate = request.ToDate;
            ViewBag.CategoryId = request.CategoryID;
            ViewBag.SortBy = request.SortBy;
            ViewBag.SortDirection = request.SortDirection;

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

                await _expenseValidator.ValidateInputAndUserExist(request);

                int? categoryId = await _expenseValidator.ValidateCategoryID(request.CategoryID);

                if (categoryId == null || categoryId == 0)
                {
                    throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);
                }

                await _expenseService.CreateAsync(request);

                TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseCreatedSuccess;

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
        public async Task<IActionResult> Edit(int expenseID,ExpenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExpenseId = expenseID;

                ViewBag.Categories =
                    await _categoryService.GetAllAsync();

                return View(request);
            }

            try
            {
                await _expenseValidator.ValidateInputAndUserExist(request);

                int? categoryId = await _expenseValidator.ValidateCategoryID(request.CategoryID);

                if(categoryId == null || categoryId == 0)
                {
                    throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);
                }

                request.CategoryID = categoryId;

                var updated = await _expenseService.UpdateAsync(expenseID,request);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseEditedSuccess;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                ViewBag.ExpenseId = expenseID;

                ViewBag.Categories = await _categoryService.GetAllAsync();

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

                TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseDeletedSuccess;

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