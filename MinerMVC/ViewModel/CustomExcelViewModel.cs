using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.ViewModel;

public class CustomExcelViewModel
{
    public CustomExcelViewModel(CustomExcel customExcel)
    {
        Description = customExcel.Description;
        Id = customExcel.Id;
        Name = customExcel.Name;
        Verified = customExcel.Verified;
        ImageName = customExcel.ImageName;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageName { get; set; }
    public IFormFile? Image { get; set; }
    public bool Verified { get; set; }
}