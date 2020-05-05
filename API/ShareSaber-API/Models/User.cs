using MongoDB.Bson;
using ShareSaber_API.Types;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ShareSaber_API.Models
{
    public class User
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId), JsonIgnore]
        public string Id { get; set; }

        public string DiscordID { get; set; }

        public bool Banned { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ShareSaberRole Role { get; set; }
    }
}
