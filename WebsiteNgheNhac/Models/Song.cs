using System.ComponentModel.DataAnnotations;

namespace WebsiteNgheNhac.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string SongTitle { get; set; } = default!;

        [Required, MaxLength(300)]
        public string AudioUrl { get; set; } = default!;

        public TimeSpan? Duration { get; set; }

        public int ArtistId { get; set; }

        public Artist Artist { get; set; } = default!;

        public int? AlbumId { get; set; }

        public Album? Album { get; set; }

        public int? GenreId { get; set; }

        public Genre? Genre { get; set; }

        public int PlayCount { get; set; } = 0;

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
