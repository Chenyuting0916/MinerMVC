using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinerMVC.Models;

public class CustomExcel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [NotMapped]
    public IFormFile Image { get; set; }
}