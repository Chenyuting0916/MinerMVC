namespace MinerMVC.Models.TodoList;

public class TodoList
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Status { get; set; }
}