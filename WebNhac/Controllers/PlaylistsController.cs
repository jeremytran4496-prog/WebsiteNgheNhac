using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebNhac.Data;
using WebNhac.Models;

namespace WebNhac.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlaylistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Playlists.ToListAsync());
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Add(int songId)
        {
            var song = await _context.Songs.Include(s=>s.Artist).FirstOrDefaultAsync(s => s.Id == songId);
            if (song == null) return NotFound();

            var vm = new AddToPlaylistViewModel
            {
                SongId = songId,
                SongTitle = $"{song.Title} - {song.Artist?.Name}",
                Playlists = await _context.Playlists.OrderBy(p => p.Name).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToPlaylistViewModel vm)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == vm.SongId);
            var playlist = await _context.Playlists.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == vm.PlaylistId);
            if (song == null || playlist == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy bài hát hoặc playlist");
            }

            if(!ModelState.IsValid)
            {
                vm.Playlists = await _context.Playlists.OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            var existed = await _context.PlaylistItems
                .AnyAsync(pi => pi.PlaylistId == vm.PlaylistId && pi.SongId == vm.SongId);
            if (!existed)
            {
                var nextOrder = await _context.PlaylistItems
                    .Where(pi => pi.PlaylistId == vm.PlaylistId)
                    .Select(pi => (int?)pi.Order).MaxAsync() ?? 0;

                _context.PlaylistItems.Add(new PlaylistItem
                {
                    PlaylistId = vm.PlaylistId,
                    SongId = vm.SongId,
                    Order = nextOrder + 1
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Player", new { playlistId  = vm.PlaylistId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int playlistId, int itemId)
        {
            var item = await _context.PlaylistItems
                .FirstOrDefaultAsync(pi => pi.Id == itemId && pi.PlaylistId == playlistId);
            if (item == null) return NotFound();

            _context.PlaylistItems.Remove(item);
            await _context.SaveChangesAsync();

            var remain = await _context.PlaylistItems
                .Where(pi => pi.PlaylistId == playlistId)
                .OrderBy(pi => pi.Order).ToListAsync();

            for (int i = 0; i< remain.Count; i++)
            {
                remain[i].Order = i + 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Player", new { playlistId });
        }


        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }
    }
}
