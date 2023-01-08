using Microsoft.EntityFrameworkCore;
using MinerMVC.Data;
using MinerMVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomExcelDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<TodoListDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TodoList");
    options.UseSqlServer(connectionString);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJobCrawlerService, JobCrawlerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();