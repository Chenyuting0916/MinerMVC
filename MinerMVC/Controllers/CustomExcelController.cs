using Microsoft.AspNetCore.Mvc;
using MinerMVC.Models.Request;
using MinerMVC.Services;
using MinerMVC.Services.Image;
using MinerMVC.ViewModel;

namespace MinerMVC.Controllers;

public class CustomExcelController : Controller
{
    private readonly ICustomExcelService _customExcelService;
    private readonly IImageService _imageService;

    public CustomExcelController(ICustomExcelService customExcelService, IImageService imageService)
    {
        _customExcelService = customExcelService;
        _imageService = imageService;
    }

    public IActionResult Index()
    {
        var model = _customExcelService.GetAll().Select(x => new CustomExcelViewModel(x)).ToList();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Insert(CustomExcelRequest customExcelRequest)
    {
        customExcelRequest.ImageName = await _imageService.AddImage(customExcelRequest.Image);
        _customExcelService.Insert(customExcelRequest.ToCustomExcel());
        return RedirectToAction("Index", "CustomExcel");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        _imageService.DeleteImage(_customExcelService.Get(id).ImageName);
        _customExcelService.Delete(id);
        return RedirectToAction("Index", "CustomExcel");
    }

    [HttpPost]
    public IActionResult Edit(CustomExcelRequest customExcelRequest)
    {
        _customExcelService.Edit(customExcelRequest.ToCustomExcel());
        _imageService.EditImage();
        return RedirectToAction("Index", "CustomExcel");
    }
}