using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareSaber.Models
{
    public class BSFile
    {
        public string Id { get; set; }
        public FileType Type { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool UnlimitedDownloads { get; set; }
        public bool HasPassword { get; set; }
        public string Password { get; set; }
        public int Downloads { get; set; }
        public int MaxDownloads { get; set; }
        public string Uploader { get; set; }
        public string DownloadURL { get; set; }
        public DateTime Uploaded { get; set; }
    }

    public enum FileType
    {
        Map
    }
}
