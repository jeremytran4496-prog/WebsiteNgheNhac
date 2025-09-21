using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebNhac.Data;
using WebNhac.Models;

namespace WebNhac.Controllers
{
    public class UploadOptions
    {
        public string AudioFolder { get; set; } = "wwwroot/media/audio";
        public string CoverFolder { get; set; } = "wwwroot/images/covers";
        public string[] AllowedAudioExtensions { get; set; } = new[] { ".mp3", ".m4a", ".wav" };
        public string[] AllowedImageExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        public int MaxAudioMB { get; set; } = 50;
        public int MaxImageMB { get; set; } = 5;
    }


    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UploadOptions _opt;


        public SongsController(ApplicationDbContext context, IWebHostEnvironment env, IOptions<UploadOptions> opt)
        {
            _context = context;
            _env = env;
            _opt = opt.Value;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Songs.Include(s => s.Album).Include(s => s.Artist).Include(s => s.Genre);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            // Lấy liên quan: ưu tiên cùng Nghệ sĩ, thiếu thì bổ sung theo Thể loại
            var related = await _context.Songs
                .Include(s => s.Artist)
                .Where(s => s.Id != song.Id && s.ArtistId == song.ArtistId)
                .OrderByDescending(s => s.PlayCount)
                .ThenByDescending(s => s.CreatedAt)
                .Take(6)
                .ToListAsync();

            if (related.Count < 6 && song.GenreId != null)
            {
                var need = 6 - related.Count;
                var more = await _context.Songs
                    .Include(s => s.Artist)
                    .Where(s => s.Id != song.Id
                                && s.GenreId == song.GenreId
                                && s.ArtistId != song.ArtistId)
                    .OrderByDescending(s => s.PlayCount)
                    .ThenByDescending(s => s.CreatedAt)
                    .Take(need)
                    .ToListAsync();
                related.AddRange(more);
            }

            var vm = new SongDetailsViewModel { Song = song, Related = related };

            return View(vm);
        }

        // GET: Songs/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Title");
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Song song, IFormFile audioFile, IFormFile? coverFile)
        {
            // 1) Validate bắt buộc có audio
            if (audioFile == null || audioFile.Length == 0)
                ModelState.AddModelError("AudioUrl", "Hãy chọn file âm thanh.");

            // 2) Validate định dạng & kích thước
            string audioExt = Path.GetExtension(audioFile?.FileName ?? "").ToLowerInvariant();
            if (!_opt.AllowedAudioExtensions.Contains(audioExt))
                ModelState.AddModelError("AudioUrl", "Định dạng audio không hỗ trợ.");

            if (audioFile != null && audioFile.Length > _opt.MaxAudioMB * 1024L * 1024L)
                ModelState.AddModelError("AudioUrl", $"File audio vượt quá {_opt.MaxAudioMB}MB.");

            string? coverExt = null;
            if (coverFile != null)
            {
                coverExt = Path.GetExtension(coverFile.FileName).ToLowerInvariant();
                if (!_opt.AllowedImageExtensions.Contains(coverExt))
                    ModelState.AddModelError("CoverImageUrl", "Định dạng ảnh không hỗ trợ.");
                if (coverFile.Length > _opt.MaxImageMB * 1024L * 1024L)
                    ModelState.AddModelError("CoverImageUrl", $"Ảnh vượt quá {_opt.MaxImageMB}MB.");
            }

            ModelState.Remove(nameof(Song.AudioUrl));       
            ModelState.Remove(nameof(Song.CoverImageUrl));
            ModelState.Remove(nameof(Song.Artist));


            if (!ModelState.IsValid)
            {
                ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
                ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Title", song.AlbumId);
                ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", song.GenreId);
                return View(song);
            }

            // 3) Lưu file vật lý với tên ngẫu nhiên
            Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _opt.AudioFolder));
            var audioName = $"{Guid.NewGuid():N}{audioExt}";
            var audioPath = Path.Combine(_env.ContentRootPath,_opt.AudioFolder, audioName);
            using (var stream = System.IO.File.Create(audioPath))
                await audioFile.CopyToAsync(stream);

            var f = TagLib.File.Create(Path.Combine(_env.ContentRootPath, audioPath)); // audioPath là đường dẫn vật lý
            var seconds = (int)f.Properties.Duration.TotalSeconds;
            song.Duration = TimeSpan.FromSeconds(seconds);

            // 4) Lưu ảnh bìa (nếu có)
            string? coverRel = null;
            if (coverFile != null)
            {
                Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _opt.CoverFolder));
                var coverName = $"{Guid.NewGuid():N}{coverExt}";
                var coverPath = Path.Combine(_env.ContentRootPath,_opt.CoverFolder, coverName);
                using (var stream = System.IO.File.Create(coverPath))
                    await coverFile.CopyToAsync(stream);

                coverRel = "/" + _opt.CoverFolder.Replace("wwwroot", "").Trim('/', '\\') + "/" + coverName;
            }

            // 5) Gán đường dẫn tương đối để client truy cập
            var audioRel = "/" + _opt.AudioFolder.Replace("wwwroot", "").Trim('/', '\\') + "/" + audioName;
            song.AudioUrl = audioRel;
            if (coverRel != null) song.CoverImageUrl = coverRel;

            song.CreatedAt = DateTime.UtcNow;
            _context.Add(song);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 

        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Title", song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AudioUrl,CoverImageUrl,ArtistId,Duration,AlbumId,GenreId,PlayCount,CreatedAt,Lyrics")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(Song.AudioUrl));
            ModelState.Remove(nameof(Song.CoverImageUrl));
            ModelState.Remove(nameof(Song.Artist));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Title", song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
            }

            await _context.SaveChangesAsync();  


            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
