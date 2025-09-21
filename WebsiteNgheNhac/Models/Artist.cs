using System.ComponentModel.DataAnnotations;

namespace WebsiteNgheNhac.Models
{
    public class Artist
    {
        public int Id { get; set; }


        [Required, MaxLength(200)]
        public string ArtistName { get; set; } = default!;

        [MaxLength(500)]
        public string? ArtistBio {  get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
