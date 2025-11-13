using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarryJBriggs.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet("/login")] public IActionResult Index() => View();

        [ValidateAntiForgeryToken]
        [HttpPost("/login")]
        public async Task<IActionResult> Index(string email, string password)
        {
            var ownerEmail = Environment.GetEnvironmentVariable("OWNER_EMAIL");
            var ownerPass = Environment.GetEnvironmentVariable("OWNER_PASSWORD");
            if (email == ownerEmail && password == ownerPass)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.Role, "Owner") };
                var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(new ClaimsPrincipal(id));
                return Redirect("/");
            }
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return Redirect("/"); }


    }
}
