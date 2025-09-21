using System.ComponentModel.DataAnnotations;

namespace WebNhac.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Display(Name ="Tên bài hát")]
        [Required, MaxLength(250)]
        public string Title { get; set; } = default!;

        [Display(Name = "Đường dẫn bài hát")]
        [Required,MaxLength(300)]
        public string AudioUrl { get; set; } = default!;

        [Display(Name = "Đường dẫn ảnh bìa")]
        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Thời lượng")]
        public TimeSpan? Duration { get; set; }

        
        public int ArtistId { get; set; }

        [Display(Name = "Ca sĩ")]
        public Artist Artist { get; set; } = default!;

        [Display(Name = "Album")]
        public int? AlbumId { get; set; }

        [Display(Name = "Album")]
        public Album? Album { get; set; }

        [Display(Name = "Thể loại")]
        public int? GenreId { get; set; }

        [Display(Name = "Thể loại")]
        public Genre? Genre { get; set; }

        [Display(Name = "Số lượt nghe")]
        public int PlayCount { get; set; } = 0;

        [Display(Name = "Ngày tải lên")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Lời")]
        [DataType(DataType.MultilineText)]
        public string? Lyrics { get; set; }
    }
}
