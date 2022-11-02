using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers;

public class DillyController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}