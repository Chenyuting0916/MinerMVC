using Microsoft.EntityFrameworkCore;
using MinerMVC.Data;
using MinerMVC.Services;
using MinerMVC.Services.Image;
using MinerMVC.Services.PdfMerge;
using MinerMVC.Services.Transcription;

var builder = WebApplication.CreateBuilder(args);

// 設置默認編碼為UTF-8
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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
builder.Services.AddScoped<IPdfMergeService, PdfMergeService>();
builder.Services.AddScoped<PdfAdapter>();
builder.Services.AddScoped<ITranscriptionService, VoskTranscriptionService>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

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