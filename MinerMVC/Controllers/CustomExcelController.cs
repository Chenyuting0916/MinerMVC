using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinerMVC.Models.Request;
using MinerMVC.Security;
using MinerMVC.Services;
using MinerMVC.Services.Image;
using MinerMVC.ViewModel;

namespace MinerMVC.Controllers;

[Authorize]
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

    [HttpGet]
    public IActionResult Image(string name)
    {
        var path = _imageService.TryResolveImagePath(name);
        if (path == null)
            return NotFound();

        return PhysicalFile(path, UploadRules.ImageContentType(path));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Insert(CustomExcelRequest customExcelRequest)
    {
        var rejected = await TryAssignImageAsync(customExcelRequest, editing: false);
        if (rejected != null)
            return rejected;

        _customExcelService.Insert(customExcelRequest.ToCustomExcel());
        return RedirectToAction("Index", "CustomExcel");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _imageService.DeleteImage(_customExcelService.Get(id).ImageName);
        _customExcelService.Delete(id);
        return RedirectToAction("Index", "CustomExcel");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomExcelRequest customExcelRequest)
    {
        var rejected = await TryAssignImageAsync(customExcelRequest, editing: true);
        if (rejected != null)
            return rejected;

        _customExcelService.Edit(customExcelRequest.ToCustomExcel());
        return RedirectToAction("Index", "CustomExcel");
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public void UpdateVerifiedStatus(int id, bool status)
    {
        _customExcelService.UpdateVerifiedStatus(id, status);
    }

    private async Task<IActionResult?> TryAssignImageAsync(CustomExcelRequest request, bool editing)
    {
        try
        {
            request.ImageName = editing
                ? await _imageService.EditImage(request.Image, request.ImageName ?? "default.png")
                : await _imageService.AddImage(request.Image);
            return null;
        }
        catch (InvalidOperationException)
        {
            TempData["Error"] = "只接受 jpg、jpeg、png、gif 或 webp 圖片，且大小不可超過 5 MB。";
            return RedirectToAction(nameof(Index));
        }
    }
}
