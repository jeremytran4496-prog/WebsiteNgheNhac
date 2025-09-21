using System.ComponentModel.DataAnnotations;

namespace WebNhac.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Display(Name ="Tên Album")]
        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [Display(Name = "Ca sĩ")]
        public int ArtistId { get; set; }

        [Display(Name = "Ca sĩ")]
        public Artist Artist { get; set; } = default!;

        [Display(Name = "Ngày phát hành")]
        public DateTime? ReleaseDate { get; set; }


        [Display(Name = "Đường dẫn ảnh bìa")]
        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }


        
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
