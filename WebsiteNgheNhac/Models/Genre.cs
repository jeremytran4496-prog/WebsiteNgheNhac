using System.ComponentModel.DataAnnotations;

namespace WebsiteNgheNhac.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string GenreName { get; set; } = default!;

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
