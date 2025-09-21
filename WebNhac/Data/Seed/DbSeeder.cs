using Microsoft.EntityFrameworkCore;
using WebNhac.Models;

namespace WebNhac.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db, IWebHostEnvironment env)
        {
            if (!await db.Genres.AnyAsync())
            {
                db.Genres.AddRange(
                    new Genre { Name = "Nhạc Pop" },
                    new Genre { Name = "Nhac Trung" },
                    new Genre { Name = "Nhạc không lời" }
                );
            }

            if (!await db.Artists.AnyAsync())
            {
                db.Artists.AddRange(
                    new Artist { Name = "Hòa Minzy" },
                    new Artist { Name = "Nguyễn Hùng" },
                    new Artist { Name = "Trung Quân" }
                );
            }

            await db.SaveChangesAsync();

            if (!await db.Albums.AnyAsync())
            {
                var artist = await db.Artists.FirstAsync();
                db.Albums.Add(new Album { Title = "Nỗi đau giữa hòa bình", ArtistId = artist.Id, ReleaseDate = new DateTime(2025, 9, 2) });
                await db.SaveChangesAsync();
            }

            if (!await db.Songs.AnyAsync())
            {
                var artist = await db.Artists.FirstAsync();
                var album = await db.Albums.FirstAsync();
                var pop = await db.Genres.FirstAsync(g => g.Name == "Nhạc Pop");

                
                db.Songs.AddRange(
                    new Song { Title = "Nỗi đau giữa hòa bình", ArtistId = artist.Id, AlbumId = album.Id, GenreId = pop.Id, AudioUrl = "/media/audio/Noi dau giua hoa binh.mp3", Duration = TimeSpan.FromSeconds(210) },
                    new Song { Title = "Điều buồn nhất khi yêu", ArtistId = artist.Id, GenreId = pop.Id, AudioUrl = "/media/audio/Dieu buon nhat khi yeu.mp3", Duration = TimeSpan.FromSeconds(185) }
                    
                );
                await db.SaveChangesAsync();
            }

            if (!await db.Playlists.AnyAsync())
            {
                var playlist = new Playlist { Name = "Mưa đỏ" };
                db.Playlists.Add(playlist);
                await db.SaveChangesAsync();

                var songs = await db.Songs.OrderBy(s => s.Id).ToListAsync();
                int order = 1;
                foreach (var s in songs)
                {
                    db.PlaylistItems.Add(new PlaylistItem { PlaylistId = playlist.Id, SongId = s.Id, Order = order++ });
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
