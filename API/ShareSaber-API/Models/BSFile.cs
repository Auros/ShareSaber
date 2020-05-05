using MongoDB.Bson;
using ShareSaber_API.Types;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System;

namespace ShareSaber_API.Models
{
    public class BSFile
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId), JsonIgnore]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public FileType Type { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool UnlimitedDownloads { get; set; }
        public bool HasPassword { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public int Downloads { get; set; }
        public int MaxDownloads { get; set; }
        public string Uploader { get; set; }
        public string DownloadURL { get; set; }
        public DateTime Uploaded { get; set; }
    }
}
