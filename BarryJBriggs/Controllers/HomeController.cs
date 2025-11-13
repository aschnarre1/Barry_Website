using BarryJBriggs.Data;
using BarryJBriggs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BarryJBriggs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDb _db;

        public HomeController(ILogger<HomeController> logger, AppDb db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index([FromServices] AppDb db)
        {
            ViewBag.Companies = await db.Companies.OrderBy(c => c.SortOrder).ToListAsync();
            return View();
        }
        public async Task<IActionResult> About([FromServices] AppDb db)
        {
            var page = await db.AboutPages.FirstOrDefaultAsync(p => p.Slug == "about");
            return View(page);
        }
        public IActionResult Contact() => View();

        public async Task<IActionResult> Works()
        {
            var sections = await _db.Sections
                .Include(s => s.Items)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return View(sections);  
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}