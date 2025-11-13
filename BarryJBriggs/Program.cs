using BarryJBriggs.Data;
using BarryJBriggs.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<EmailService>();
builder.Services.AddDbContext<AppDb>(o => o.UseSqlite("Data Source=app.db"));
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.AddResponseCompression(o =>
{
    o.Providers.Add<GzipCompressionProvider>();
    o.EnableForHttps = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var host = context.Request.Host.Value;
    if (host.StartsWith("www."))
    {
        var newHost = host.Replace("www.", "");
        var newUrl = $"https://{newHost}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(newUrl, permanent: true);
        return;
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
