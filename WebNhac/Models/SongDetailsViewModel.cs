namespace WebNhac.Models
{
    public class SongDetailsViewModel
    {
        public Song Song { get; set; } = default!;
        public List<Song> Related { get; set; } = new();
    }
}
