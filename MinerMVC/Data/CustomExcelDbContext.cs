using Microsoft.EntityFrameworkCore;
using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.Data;

public class CustomExcelDbContext : DbContext
{
    public DbSet<CustomExcel> CustomExcels { get; set; }
    public CustomExcelDbContext(DbContextOptions<CustomExcelDbContext> options) : base(options)
    {
        
    }
}