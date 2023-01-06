using Microsoft.AspNetCore.Mvc;
using MinerMVC.Data;
using MinerMVC.Models;

namespace MinerMVC.Controllers;

public class CustomExcelController : Controller
{
    private readonly CustomExcelDbContext _customExcelDbContext;

    public CustomExcelController(CustomExcelDbContext customExcelDbContext)
    {
        _customExcelDbContext = customExcelDbContext;
    }

    public IActionResult Index()
    {
        var model = _customExcelDbContext.CustomExcels.First();
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(CustomExcel model)
    {
        if (!ModelState.IsValid) return View(model);
        // Check if image was uploaded
        if (model.Image.Length > 0)
        {
            // Process image
        }

        // Save form data

        _customExcelDbContext.CustomExcels.Add(model);
        _customExcelDbContext.SaveChanges();


        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}