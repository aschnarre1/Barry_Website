using BarryJBriggs.Services;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();


//port listener
builder.WebHost.UseUrls("http://0.0.0.0:8080");


builder.Services.AddControllersWithViews();
builder.Services.AddTransient<EmailService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
