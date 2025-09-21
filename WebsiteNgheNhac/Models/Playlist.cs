using System.ComponentModel.DataAnnotations;

namespace WebsiteNgheNhac.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string PlaylistName { get; set; } = default!;

        public ICollection<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
    }
}
