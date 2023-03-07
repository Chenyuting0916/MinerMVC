using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers
{
    public class BauchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
