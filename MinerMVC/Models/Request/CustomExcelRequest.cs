using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.Models.Request;

public class CustomExcelRequest
{
    private const string DefaultImagePath = "default.png";
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ImageName { get; set; } = DefaultImagePath;
    public IFormFile? Image { get; set; }
    public bool Verified { get; set; }
}

public static class CustomExcelRequestExtension
{
    public static CustomExcel ToCustomExcel(this CustomExcelRequest customExcelRequest)
    {
        return new CustomExcel()
        {
            Id = customExcelRequest.Id,
            Description = customExcelRequest.Description,
            Name = customExcelRequest.Name,
            ImageName = customExcelRequest.ImageName,
            Verified = customExcelRequest.Verified
        };
    }
}