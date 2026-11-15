using System.Diagnostics;
using ExpenseTracker.Features.Dashboard.Queries;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[Authorize]
public class HomeController : BaseController
{
    private readonly GetDashboardQueryHandler _getDashboard;
    private readonly ILogger<HomeController> _logger;

    public HomeController(GetDashboardQueryHandler getDashboard, ILogger<HomeController> logger)
    {
        _getDashboard = getDashboard;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _getDashboard.HandleAsync(GetCurrentUserId());
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
