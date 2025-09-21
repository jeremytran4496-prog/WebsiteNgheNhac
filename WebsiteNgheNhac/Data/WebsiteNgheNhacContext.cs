using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebsiteNgheNhac.Models;

namespace WebsiteNgheNhac.Data
{
    public class WebsiteNgheNhacContext : DbContext
    {
        public WebsiteNgheNhacContext (DbContextOptions<WebsiteNgheNhacContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Album> Album => Set<Album>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistItem> PlaylistItems => Set<PlaylistItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<PlaylistItem>()
                .HasIndex(pi => new { pi.PlaylistId, pi.Order })
                .IsUnique();

            b.Entity<PlaylistItem>()
                .HasOne(pi => pi.Playlist)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<PlaylistItem>()
                .HasOne(pi => pi.Song)
                .WithMany()
                .HasForeignKey(pi => pi.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Song>()
                .Property(s => s.Duration)
                .HasConversion(
                    v => v.HasValue ? v.Value.Ticks : 0L,
                    v => v == 0 ? (TimeSpan?)null : TimeSpan.FromTicks(v));
        }
    }
}
