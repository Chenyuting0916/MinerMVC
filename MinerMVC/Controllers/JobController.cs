using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers;

public class JobController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}