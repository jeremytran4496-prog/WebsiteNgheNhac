namespace WebNhac.Models
{
    public class AddToPlaylistViewModel
    {
        public int SongId { get; set; }
        public int PlaylistId { get; set; }

        
        public string? SongTitle { get; set; }
        public List<Playlist>? Playlists { get; set; }
    }
}
