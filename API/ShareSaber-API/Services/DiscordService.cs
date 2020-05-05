using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ShareSaber_API.Models;
using System.Threading.Tasks;
using Web = System.Net.HttpWebRequest;
using ShareSaber_API.Types;

namespace ShareSaber_API.Services
{
    public class DiscordService
    {
        public string ID { get; }
        public string Secret { get; }
        public string RedirectURL { get; }
        public string ReleaseRedirectURL { get; set; }
        public string Token { get; }

        public DiscordService(IDiscordAuthenticationSettings settings)
        {
            ID = settings.ID;
            Secret = settings.Secret;
            RedirectURL = settings.RedirectURL;
            ReleaseRedirectURL = settings.ReleaseRedirectURL;

#if RELEASE
            RedirectURL = ReleaseRedirectURL;
#endif
            Token = settings.Token;
        }

        public async Task<string> SendDiscordOAuthRequestViaAuthCode(string code)
        {
            string authstring = $"https://discordapp.com/api/oauth2/token";

            Web webReq = (Web)WebRequest.Create(authstring);
            webReq.Method = "POST";
            string parameters = $"client_id={ID}&client_secret={Secret}&grant_type=authorization_code&code={code}&redirect_uri={RedirectURL}";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = byteArray.Length;
            Stream postStream = await webReq.GetRequestStreamAsync();
            await postStream.WriteAsync(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = await webReq.GetResponseAsync();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFS = await reader.ReadToEndAsync();
            string tokenInfo = responseFS.Split(',')[0].Split(':')[1];
            string access_token = tokenInfo.Trim().Substring(1, tokenInfo.Length - 3);
            return access_token;
        }

        public async Task<DiscordUser> GetDiscordUserProfile(string access_token)
        {
            Web webReq = (Web)WebRequest.Create("https://discordapp.com/api/users/@me");
            webReq.Method = "Get";
            webReq.ContentLength = 0;
            webReq.Headers.Add("Authorization", "Bearer " + access_token);
            webReq.ContentType = "application/x-www-form-urlencoded";
            var response1 = await webReq.GetResponseAsync();
            StreamReader reader1 = new StreamReader(((HttpWebResponse)response1).GetResponseStream());
            string apiResponse1 = await reader1.ReadToEndAsync();

            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>(apiResponse1);
            return user;
        }

        public async Task<DiscordUser> GetDiscordUser(string id)
        {
            Web webReq = (Web)WebRequest.Create("https://discordapp.com/api/v6/users/" + id);
            webReq.Method = "Get";
            webReq.ContentLength = 0;
            webReq.Headers.Add("Authorization", "Bot " + Token);
            webReq.ContentType = "application/x-www-form-urlencoded";
            var response1 = await webReq.GetResponseAsync();
            StreamReader reader1 = new StreamReader(((HttpWebResponse)response1).GetResponseStream());
            string apiResponse1 = await reader1.ReadToEndAsync();

            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>(apiResponse1);
            return user;
        }
    }
}
