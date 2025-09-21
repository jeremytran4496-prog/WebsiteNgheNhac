using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebNhac.Data;
using WebNhac.Models;

namespace WebNhac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                NewSongs = await _db.Songs
                    .Include(s => s.Artist)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(12).ToListAsync(),

                TopSongs = await _db.Songs
                    .Include(s => s.Artist)
                    .OrderByDescending(s => s.PlayCount)
                    .Take(12).ToListAsync(),

                Playlists = await _db.Playlists
                    .Include(p => p.Items).ThenInclude(i => i.Song)
                    .OrderByDescending(p => p.Items.Count)
                    .Take(6).ToListAsync()
            };
            return View(vm);
        }

        // Tìm kiếm nhanh
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            q = q?.Trim() ?? "";
            var results = await _db.Songs
                .Include(s => s.Artist)
                .Where(s => s.Title.Contains(q) || s.Artist.Name.Contains(q))
                .OrderBy(s => s.Title)
                .Take(50)
                .ToListAsync();

            ViewBag.Query = q;
            return View(results);
        }
    }
}
