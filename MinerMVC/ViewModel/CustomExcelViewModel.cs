namespace MinerMVC.ViewModel;

public class CustomExcelViewModel
{
    private const string DefaultImagePath = "default.png";
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ImagePath { get; set; } = DefaultImagePath;
    public IFormFile? Image { get; set; }
    public bool Verified { get; set; }
}