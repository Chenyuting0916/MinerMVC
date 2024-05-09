namespace MinerMVC.Models.NewFolder
{
    public class DailyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
    }
}
