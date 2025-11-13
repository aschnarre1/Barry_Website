using BarryJBriggs.Data;
using BarryJBriggs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BarryJBriggs.Controllers
{
    [Authorize]
    public class AdminPagesController : Controller
    {
        private readonly AppDb _db;
        public AdminPagesController(AppDb db) { _db = db; }

        [HttpGet("/admin/pages/about")]
        public async Task<IActionResult> EditAbout()
        {
            var page = await _db.AboutPages.FirstOrDefaultAsync(p => p.Slug == "about")
                       ?? new AboutPage { Slug = "about", Html = "" };
            return View(page);
        }

        [HttpPost("/admin/pages/about")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbout(int id, string title, string html)
        {
            var page = id == 0
              ? (await _db.AboutPages.FirstOrDefaultAsync(p => p.Slug == "about")) ?? new AboutPage { Slug = "about" }
              : await _db.AboutPages.FindAsync(id);

            page.Html = html;
            page.UpdatedUtc = DateTime.UtcNow;

            if (page.Id == 0) _db.AboutPages.Add(page);
            await _db.SaveChangesAsync();
            return RedirectToAction("About", "Home");
        }

    }
}
