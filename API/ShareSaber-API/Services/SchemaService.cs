using System.Linq;
using MongoDB.Driver;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Schema;

namespace ShareSaber_API.Services
{
    public class SchemaService
    {
        public JSchema DifficultySchema { get; }
        public JSchema InfoSchema { get; }

        public SchemaService()
        {
            DifficultySchema = JSchema.Parse(GetResource("ShareSaber_API.Schemas.difficulty.schema.json"));
            InfoSchema = JSchema.Parse(GetResource("ShareSaber_API.Schemas.info.schema.json"));
        }

        public string GetResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;

            resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));

            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
