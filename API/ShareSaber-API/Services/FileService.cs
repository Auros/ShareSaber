using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Driver;
using ShareSaber_API.Models;

namespace ShareSaber_API.Services
{
    public class FileService
    {
        private readonly IMongoCollection<BSFile> _files;

        public FileService(IShareSaberDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _files = database.GetCollection<BSFile>(settings.FilesCollection);
        }

        public string GetNextKey()
        {
            BSFile file = _files.Find(f => true).ToList().LastOrDefault();
            if (file == null)
                return "1";
            return (int.Parse(file.Key, NumberStyles.HexNumber) + 1).ToString("X").ToLower();
            
        }

        public BSFile Get(string key) =>
            _files.Find(f => f.Key == key).FirstOrDefault();

        public List<BSFile> GetFilesUploadedByUser(User user) =>
            _files.Find(f => f.Uploader == user.DiscordID).ToList();
        

        public BSFile Create(BSFile file)
        {
            _files.InsertOne(file);
            return file;
        }

        public void Update(string key, BSFile file)
        {
            _files.ReplaceOne(f => f.Key == key, file);
        }
    }
}
