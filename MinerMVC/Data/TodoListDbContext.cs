using Microsoft.EntityFrameworkCore;
using MinerMVC.Models.TodoList;

namespace MinerMVC.Data;

public class TodoListDbContext : DbContext
{
    public DbSet<TodoList> TodoList { get; set; }

    public TodoListDbContext(DbContextOptions<TodoListDbContext> options) : base(options)
    {
    }
}