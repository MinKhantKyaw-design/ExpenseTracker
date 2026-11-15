using ExpenseTracker.Common.Constants;
using ExpenseTracker.DTOs.Requests;
using ExpenseTracker.Features.Expenses;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Interfaces.Categories;
using ExpenseTracker.Interfaces.Expenses;
using ExpenseTracker.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpensesController : BaseController
    {
        private readonly IExpenseCommandHandler _expenseCommandHandler;

        private readonly IExpenseQueryHandler _expenseQueryHandler;
        private readonly ICategoryQueryHandler _categoryQueryHandler;
        private readonly ExpenseValidator _expenseValidator;

        public ExpensesController(
            IExpenseCommandHandler expenseCommandHandler,
            IExpenseQueryHandler expenseQueryHandler,
            ICategoryQueryHandler categoryQueryHandler,
            ExpenseValidator expenseValidator)
        {
            _expenseCommandHandler = expenseCommandHandler;
            _expenseQueryHandler = expenseQueryHandler;
            _categoryQueryHandler = categoryQueryHandler;
            _expenseValidator = expenseValidator;
        }

        // GET: /Expenses
        public async Task<IActionResult> Index(GetExpenseListRequest request)
        {
            request.UserID = GetCurrentUserId();

            var expenses = await _expenseQueryHandler.GetAllAsync(request);

            ViewBag.Categories = await _categoryQueryHandler.GetAllAsync();

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
                await _categoryQueryHandler.GetAllAsync();

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
                    await _categoryQueryHandler.GetAllAsync();

                return View(request);
            }

            try
            {
                request.UserID = GetCurrentUserId();

                await _expenseValidator.ValidateInputAndUserExist(request);

                int? categoryId = await _expenseValidator.ValidateCategoryID(request.CategoryID);

                if (categoryId == null || categoryId == 0)
                {
                    throw new ArgumentException(ExpenseDataMessage.CategoryDoesNotExistError);
                }

                request.CategoryID = categoryId;

                await _expenseCommandHandler.CreateAsync(request);

                TempData["SuccessMessage"] = ExpenseDataMessage.ExpenseCreatedSuccess;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                ViewBag.Categories =
                    await _categoryQueryHandler.GetAllAsync();

                return View(request);
            }
        }

        // GET: /Expenses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var expense =
                await _expenseQueryHandler.GetByIdAsync(id);

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
                await _categoryQueryHandler.GetAllAsync();

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
                    await _categoryQueryHandler.GetAllAsync();

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

                var updated = await _expenseCommandHandler.UpdateAsync(expenseID,request);

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

                ViewBag.Categories = await _categoryQueryHandler.GetAllAsync();

                return View(request);
            }
        }

        // GET: /Expenses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var expense =
                await _expenseQueryHandler.GetByIdAsync(id);

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
                    await _expenseCommandHandler.DeleteAsync(id);

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