using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {

        private readonly IDashboardService _dashboardService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            var model = new DashboardViewModel
            {
                TodayTotal =
                    await _dashboardService
                        .GetTodayTotalAsync(userId),

                WeekTotal =
                    await _dashboardService
                        .GetThisWeekTotalAsync(userId),

                MonthTotal =
                    await _dashboardService
                        .GetThisMonthTotalAsync(userId),

                CategorySummary =
                    await _dashboardService
                        .GetCategorySummaryAsync(userId)
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
