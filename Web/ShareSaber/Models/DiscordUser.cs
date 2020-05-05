namespace ShareSaber.Models
{
    public class DiscordUser
    {
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string ID { get; set; }
        public string Avatar { get; set; }
    }
}
