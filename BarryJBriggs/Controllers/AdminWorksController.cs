using BarryJBriggs.Data;
using BarryJBriggs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarryJBriggs.Controllers
{
    [Authorize]

    public class AdminWorksController : Controller
    {
        private readonly AppDb _db;
        private readonly IWebHostEnvironment _env;
        public AdminWorksController(AppDb db, IWebHostEnvironment env) { _db = db; _env = env; }

        [HttpGet("/admin/works/add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Sections = await _db.Sections.OrderBy(s => s.SortOrder).ToListAsync();
            return View();
        }

        [HttpPost("/admin/works/add")]
        public async Task<IActionResult> Add(int sectionId, string caption, string youtubeUrl, IFormFile image)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            var name = $"{Guid.NewGuid():N}{ext}";
            await using (var fs = System.IO.File.Create(Path.Combine(uploads, name)))
                await image.CopyToAsync(fs);

            _db.WorkItems.Add(new WorkItem
            {
                SectionId = sectionId,
                Caption = caption,
                YoutubeId = ExtractYoutubeId(youtubeUrl),
                ImageUrl = $"/uploads/{name}"
            });
            await _db.SaveChangesAsync();
            return Redirect("/home");
        }

        private static string ExtractYoutubeId(string url)
        {
            var u = new Uri(url);
            var q = System.Web.HttpUtility.ParseQueryString(u.Query);
            return q["v"] ?? u.Segments.Last().Trim('/');
        }

        [HttpGet("/admin/works")]
        public async Task<IActionResult> List()
        {
            var items = await _db.WorkItems.Include(w => w.Section).OrderByDescending(w => w.CreatedUtc).ToListAsync();
            ViewBag.Companies = await _db.Companies.OrderBy(c => c.SortOrder).ToListAsync();
            return View(items);
        }

        [HttpPost("/admin/works/{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.WorkItems.FindAsync(id);
            if (item != null)
            {
                var path = Path.Combine(_env.WebRootPath, item.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                _db.WorkItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(List));
        }

        [HttpGet("/admin/companies/add")]
        public IActionResult AddCompany() => View();

        [HttpPost("/admin/companies/add")]
        public async Task<IActionResult> AddCompany(string name, int sortOrder, IFormFile logo)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var ext = Path.GetExtension(logo.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            var fname = $"{Guid.NewGuid():N}{ext}";
            await using (var fs = System.IO.File.Create(Path.Combine(uploads, fname)))
                await logo.CopyToAsync(fs);

            _db.Companies.Add(new Company { Name = name, ImageUrl = $"/uploads/{fname}", SortOrder = sortOrder });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        [HttpPost("/admin/companies/{id:int}/delete")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var c = await _db.Companies.FindAsync(id);
            if (c != null)
            {
                var path = Path.Combine(_env.WebRootPath, c.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                _db.Companies.Remove(c);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(List));
        }
    }
}
