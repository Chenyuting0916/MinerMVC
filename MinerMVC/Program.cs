using Microsoft.EntityFrameworkCore;
using MinerMVC.Data;
using MinerMVC.Services;
using MinerMVC.Services.Image;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomExcelDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomExcelDbContext"));
});

builder.Services.AddDbContext<DailyTaskDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DailyTaskDbContext"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJobCrawlerService, JobCrawlerService>();
builder.Services.AddScoped<ICustomExcelService, CustomExcelService>();
builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("Error/Error404");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CustomExcel}/{action=Index}/{id?}");

app.Run();