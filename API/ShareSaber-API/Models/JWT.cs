namespace ShareSaber_API.Models
{
    public class JWT : IJWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }

    public interface IJWT
    {
        string Key { get; set; }
        string Issuer { get; set; }
    }
}
