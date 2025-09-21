using System.ComponentModel.DataAnnotations;

namespace WebNhac.Models
{
    public class Playlist
    {
        public int Id { get; set; }


        [Display(Name ="Tên")]
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        
        public ICollection<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
    }
}
