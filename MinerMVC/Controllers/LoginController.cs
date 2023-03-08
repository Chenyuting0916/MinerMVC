using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
    }
}
