using System.ComponentModel.DataAnnotations;

namespace WebNhac.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Display(Name = "Thể loại")]
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
