using System;

namespace ShareSaber.Models
{
    public class State
    {
        public User User { get; set; }
        public DiscordUser Discord { get; set; }

        private string _token;
        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                pageUpdated?.Invoke();
            }
        }

        public Action pageUpdated;
    }
}
