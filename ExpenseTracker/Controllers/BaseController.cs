using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class BaseController : Controller
    {
        protected int GetCurrentUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}