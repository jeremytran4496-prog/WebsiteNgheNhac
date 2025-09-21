using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNhac.Data;
using WebNhac.Models;

namespace WebNhac.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PlayerController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(int playlistId = 1)
        {
            var playlist = await _db.Playlists
                .Include(p=>p.Items)
                .ThenInclude(i=>i.Song)
                .ThenInclude(s=>s.Artist)
                .FirstOrDefaultAsync(p=>p.Id == playlistId);

            if (playlist == null) return NotFound();
            return View(playlist);
        }

        // /Player/ByArtist/5
        public async Task<IActionResult> ByArtist(int artistId)
        {
            var artist = await _db.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
            if (artist == null) return NotFound();

            var tracks = await _db.Songs
                .Include(s => s.Artist)
                .Where(s => s.ArtistId == artistId)
                .OrderByDescending(s => s.PlayCount)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();

            var vm = new PlayerFlatViewModel
            {
                Title = $"Nghe theo Nghệ sĩ: {artist.Name}",
                Tracks = tracks
            };
            return View("Flat", vm);
        }

        // /Player/ByAlbum/3
        public async Task<IActionResult> ByAlbum(int albumId)
        {
            var album = await _db.Albums.Include(a => a.Artist).FirstOrDefaultAsync(a => a.Id == albumId);
            if (album == null) return NotFound();

            var tracks = await _db.Songs
                .Include(s => s.Artist)
                .Where(s => s.AlbumId == albumId)
                .OrderBy(s => s.Id) // hoặc CreatedAt/TrackNo nếu có
                .ToListAsync();

            var vm = new PlayerFlatViewModel
            {
                Title = $"Nghe Album: {album.Title} — {album.Artist.Name}",
                Tracks = tracks
            };
            return View("Flat", vm);
        }

        // /Player/ByGenre/4
        public async Task<IActionResult> ByGenre(int genreId)
        {
            var genre = await _db.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
            if (genre == null) return NotFound();

            var tracks = await _db.Songs
                .Include(s => s.Artist)
                .Where(s => s.GenreId == genreId)
                .OrderByDescending(s => s.PlayCount)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();

            var vm = new PlayerFlatViewModel
            {
                Title = $"Nghe theo Thể loại: {genre.Name}",
                Tracks = tracks
            };
            return View("Flat", vm);
        }
    }
}
