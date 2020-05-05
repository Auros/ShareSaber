namespace ShareSaber_API.Models
{
    public class ShareSaberDatabaseSettings : IShareSaberDatabaseSettings
    {
        public string AuditCollection { get; set; }
        public string FilesCollection { get; set; }
        public string UsersCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IShareSaberDatabaseSettings
    {
        string AuditCollection { get; set; }
        string FilesCollection { get; set; }
        string UsersCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
