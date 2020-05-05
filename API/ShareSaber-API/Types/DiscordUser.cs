namespace ShareSaber_API.Types
{
    public class DiscordUser
    {
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string ID { get; set; }
        private string _avatar;

        public string Avatar
        {
            get => _avatar;
            set => _avatar = "https://cdn.discordapp.com/avatars/" + ID + "/" + value + (value.Substring(0, 2) == "a_" ? ".gif" : ".png");

        }
    }
}
