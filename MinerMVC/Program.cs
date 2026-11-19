using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MinerMVC.Data;
using MinerMVC.Services;
using MinerMVC.Services.Image;
using MinerMVC.Services.PdfMerge;
using MinerMVC.Services.Transcription;

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

AddAzureConnectionStrings(builder.Configuration);

var cookieSecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;

builder.Services.AddDbContext<CustomExcelDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomExcelDbContext"));
});

builder.Services.AddDbContext<DailyTaskDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DailyTaskDbContext"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/Login";
        options.Cookie.Name = ".TimeMiner.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = cookieSecurePolicy;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".TimeMiner.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = cookieSecurePolicy;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = cookieSecurePolicy;
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Azure App Service terminates TLS and forwards the request. The instance
    // addresses are not a fixed list, so the platform headers are trusted.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
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
    if (string.IsNullOrWhiteSpace(app.Configuration["Auth:Password"]) ||
        string.IsNullOrWhiteSpace(app.Configuration["Auth:Username"]))
    {
        app.Logger.LogWarning(
            "Auth:Username or Auth:Password is empty. Custom Excel and Daily Task stay locked until those settings are configured.");
    }
}

app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "SAMEORIGIN";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "img-src 'self' data: blob:; " +
        "media-src 'self' blob:; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://unpkg.com https://fonts.googleapis.com; " +
        "font-src 'self' data: https://cdn.jsdelivr.net https://unpkg.com https://fonts.gstatic.com; " +
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "connect-src 'self'; " +
        "frame-src 'self'; " +
        "frame-ancestors 'self'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "object-src 'none'";
    headers.Remove("X-Powered-By");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseStatusCodePages(statusContext =>
{
    var http = statusContext.HttpContext;
    if (http.Response.StatusCode == StatusCodes.Status404NotFound &&
        http.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
    {
        var accept = http.Request.Headers.Accept.ToString();
        if (string.IsNullOrEmpty(accept) ||
            accept.Contains("text/html", StringComparison.OrdinalIgnoreCase) ||
            accept.Contains("*/*", StringComparison.OrdinalIgnoreCase))
        {
            http.Response.Redirect("/Error/Error404");
        }
    }

    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.StartsWith("/Contents", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    await next();
});

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CustomExcel}/{action=Index}/{id?}");

app.Run();

static void AddAzureConnectionStrings(ConfigurationManager configuration)
{
    var names = new[] { "CustomExcelDbContext", "DailyTaskDbContext" };
    var prefixes = new[] { "SQLAZURECONNSTR_", "SQLCONNSTR_", "MYSQLCONNSTR_", "CUSTOMCONNSTR_" };
    var overlay = new Dictionary<string, string?>();

    foreach (var name in names)
    {
        if (!string.IsNullOrWhiteSpace(configuration.GetConnectionString(name)))
            continue;

        foreach (var prefix in prefixes)
        {
            var value = Environment.GetEnvironmentVariable(prefix + name);
            if (string.IsNullOrWhiteSpace(value))
                continue;

            overlay[$"ConnectionStrings:{name}"] = value;
            break;
        }
    }

    if (overlay.Count > 0)
        configuration.AddInMemoryCollection(overlay);
}
