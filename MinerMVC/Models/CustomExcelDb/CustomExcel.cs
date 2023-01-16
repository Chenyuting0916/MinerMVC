using System.ComponentModel;

namespace MinerMVC.Models.CustomExcelDb;

public class CustomExcel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [DefaultValue("default.png")]
    public string? ImagePath { get; set; } 
    public bool Verified { get; set; }
}