namespace WebNhac.Models
{
    public class HomeViewModel
    {
        public List<Song> NewSongs { get; set; } = new();

        public List<Song> TopSongs { get; set; } = new();

        public List<Playlist> Playlists { get; set; } = new();
    }
}
