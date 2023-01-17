using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.ViewModel;

public class CustomExcelViewModel
{
    public CustomExcelViewModel()
    {
        
    }
    public CustomExcelViewModel(CustomExcel customExcel)
    {
        Description = customExcel.Description;
        Id = customExcel.Id;
        Name = customExcel.Name;
        Verified = customExcel.Verified;
        ImageName = customExcel.ImageName;
    }
    
    private const string DefaultImagePath = "default.png";
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ImageName { get; set; } = DefaultImagePath;
    public IFormFile? Image { get; set; }
    public bool Verified { get; set; }
}