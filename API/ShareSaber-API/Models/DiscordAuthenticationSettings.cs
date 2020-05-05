namespace ShareSaber_API.Models
{
    public class DiscordAuthenticationSettings : IDiscordAuthenticationSettings
    {
        public string ID { get; set; }
        public string Secret { get; set; }
        public string RedirectURL { get; set; }
        public string ReleaseRedirectURL { get; set; }
        public string Token { get; set; }
    }

    public interface IDiscordAuthenticationSettings
    {
        string ID { get; set; }
        string Secret { get; set; }
        string RedirectURL { get; set; }
        string ReleaseRedirectURL { get; set; }
        string Token { get; set; }
    }
}
