using System.Linq;
using MongoDB.Driver;
using ShareSaber_API.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;

namespace ShareSaber_API.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IShareSaberDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollection);
        }

        public User Get(string discordID) =>
            _users.Find(user => user.DiscordID == discordID).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string discordID, User user)
        {
            _users.ReplaceOne(u => u.DiscordID == discordID, user);
        }

        public User UserFromContext(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var discordID = claim.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault();
            return Get(discordID.Value);
        }
    }
}
