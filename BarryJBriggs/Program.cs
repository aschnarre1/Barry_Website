using BarryJBriggs.Services;
using dotenv.net;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();



builder.Services.AddControllersWithViews();
builder.Services.AddTransient<EmailService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Compression level
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);



// Add Response compression services
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseResponseCompression();

app.UseStaticFiles();

app.UseRouting();

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




app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
