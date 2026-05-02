using Microsoft.AspNetCore.Mvc;

namespace TodoApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Todo");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
