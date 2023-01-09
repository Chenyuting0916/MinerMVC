using System.ComponentModel.DataAnnotations.Schema;

namespace MinerMVC.Models.CustomExcelDb;

public class CustomExcel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte[]? Image { get; set; }
}