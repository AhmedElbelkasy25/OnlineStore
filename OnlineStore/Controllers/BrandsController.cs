using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class BrandsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
