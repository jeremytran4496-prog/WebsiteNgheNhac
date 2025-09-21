namespace WebNhac.Models
{
    public class PlayerFlatViewModel
    {
        public string Title { get; set; } = "Player";
        public List<Song> Tracks { get; set; } = new();
    }
}
