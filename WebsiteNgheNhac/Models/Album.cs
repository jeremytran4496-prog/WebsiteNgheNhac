using System.ComponentModel.DataAnnotations;

namespace WebsiteNgheNhac.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string AlbumTitle { get; set; } = default!;

        public int ArtistId { get; set; }

        public Artist Artist { get; set; } = default!;

        public DateTime? ReleaseDate { get; set; }

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
