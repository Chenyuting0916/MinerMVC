using Microsoft.AspNetCore.Mvc;
using MinerMVC.Data;
using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.Controllers;

public class CustomExcelController : Controller
{
    private readonly CustomExcelDbContext _customExcelDbContext;
    private readonly TodoListDbContext _todoListDbContext;

    public CustomExcelController(CustomExcelDbContext customExcelDbContext, TodoListDbContext todoListDbContext)
    {
        _customExcelDbContext = customExcelDbContext;
        _todoListDbContext = todoListDbContext;
    }

    public IActionResult Index()
    {
        var test = _todoListDbContext.TodoList.ToList();
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