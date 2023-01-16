using Microsoft.AspNetCore.Mvc;
using MinerMVC.Services;
using MinerMVC.ViewModel;

namespace MinerMVC.Controllers;

public class CustomExcelController : Controller
{
    private readonly ICustomExcelService _customExcelService;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public CustomExcelController(ICustomExcelService customExcelService, IWebHostEnvironment hostingEnvironment)
    {
        _customExcelService = customExcelService;
        _hostingEnvironment = hostingEnvironment;
    }

    public IActionResult Index()
    {
        var model = _customExcelService.GetAll();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Insert(CustomExcelViewModel customExcel)
    {
        var image = customExcel.Image;
        if (image != null)
        {
            var folder = Path.Combine(_hostingEnvironment.WebRootPath, "Contents");
            var imageFileName = Guid.NewGuid() + image.FileName;
            var fullPath = Path.Combine(folder, imageFileName);
            await using Stream fileStream = new FileStream(fullPath, FileMode.Create);
            await image.CopyToAsync(fileStream);
            customExcel.ImagePath = imageFileName;
        }

        _customExcelService.Insert(customExcel);
        return RedirectToAction("Index", "CustomExcel");
    }
}