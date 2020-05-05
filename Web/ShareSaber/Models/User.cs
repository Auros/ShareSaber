namespace ShareSaber.Models
{
    public class User
    {
        public string DiscordID { get; set; }
        public bool Banned { get; set; }
        public ShareSaberRole Role { get; set; }
    }
}
