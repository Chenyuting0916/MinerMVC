using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinerMVC.Models;

namespace MinerMVC.Controllers;

public class ErrorController : Controller
{

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public IActionResult Error404()
    {
        return View();
    }
}