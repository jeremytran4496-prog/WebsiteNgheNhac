namespace WebNhac.Models
{
    public class PlaylistItem
    {
        public int Id { get; set; }


        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; } = default!;


        public int SongId { get; set; }


        public Song Song { get; set; } = default!;

        
        public int Order { get; set; }
    }
}
