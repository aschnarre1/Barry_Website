using BarryJBriggs.Data;
using BarryJBriggs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BarryJBriggs.Controllers
{
    [Authorize]
    public class AdminSectionsController : Controller
    {
        private readonly AppDb _db;
        public AdminSectionsController(AppDb db) { _db = db; }

        [HttpGet("/admin/sections/add")]
        public IActionResult Add() => View();

        [HttpPost("/admin/sections/add")]
        public async Task<IActionResult> Add(string name, int sortOrder)
        {
            var slug = Regex.Replace(name.ToLower(), "[^a-z0-9]+", "-").Trim('-');
            _db.Sections.Add(new Section { Name = name, Slug = slug, SortOrder = sortOrder });
            await _db.SaveChangesAsync();
            return Redirect("/Home/Works#" + slug);
        }
    }


}
