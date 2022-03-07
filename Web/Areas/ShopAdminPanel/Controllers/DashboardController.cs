using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.ShopAdminPanel.Controllers
{
    [Area(nameof(ShopAdminPanel))]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
