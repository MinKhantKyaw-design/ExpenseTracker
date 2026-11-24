using System.Diagnostics;
using ExpenseTracker.Features.Dashboard.Queries;
using ExpenseTracker.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[Authorize]
public class HomeController : BaseController
{
    private readonly ISender _sender;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ISender sender, ILogger<HomeController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _sender.Send(new GetDashboardQuery(GetCurrentUserId()));
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
