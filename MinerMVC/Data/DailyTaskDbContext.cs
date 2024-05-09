using Microsoft.EntityFrameworkCore;
using MinerMVC.Models.NewFolder;

namespace MinerMVC.Data
{
    public class DailyTaskDbContext : DbContext
    {
        public DbSet<DailyTask> DailyTasks { get; set; }

        public DailyTaskDbContext(DbContextOptions<DailyTaskDbContext> options) : base(options)
        {

        }
    }

}
