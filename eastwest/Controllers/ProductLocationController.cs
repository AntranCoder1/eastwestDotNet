using Microsoft.AspNetCore.Mvc;

namespace eastwest.Controllers
{
    public class ProductLocationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
