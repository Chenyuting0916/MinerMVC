using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers;

public class TimerController : Controller
{
    private readonly ILogger<TimerController> _logger;

    public TimerController(ILogger<TimerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}