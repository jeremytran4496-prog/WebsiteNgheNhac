using System.ComponentModel.DataAnnotations;

namespace WebNhac.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Display(Name="Tên ca sĩ")]
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [Display(Name = "Tiểu sử")]
        [MaxLength(500)]
        public string? Bio { get; set; }

        
        public ICollection<Album> Albums { get; set; } = new List<Album>();

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
