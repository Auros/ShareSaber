using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShareSaber_API.Services;
using ShareSaber_API.Models;
using ShareSaber_API.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System;

namespace ShareSaber_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly SchemaService _schemaService;
        private readonly FileService _fileService;

        public UploadController(UserService userService, SchemaService schemaService, FileService fileService)
        {
            _userService = userService;
            _schemaService = schemaService;
            _fileService = fileService;
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Uploadt([FromForm] FileUpload body)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (user == null || user.Role == ShareSaberRole.None || body == null || body.Type == null || body.File == null || body.Name == null)
                return BadRequest();

            long size = body.File.Length;

            if (size > 15000000 || size == 0)
                return BadRequest();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            var userFolder = Path.Combine(path, user.DiscordID);
            if (!Directory.Exists(userFolder))
                Directory.CreateDirectory(userFolder);

            var nextKey = _fileService.GetNextKey();

            if (body.Type == "Map")
            {
                try
                {
                    using ZipArchive zipArchive = new ZipArchive(body.File.OpenReadStream());
                    var infoData = zipArchive.Entries.Where(x => x.Name == "info.dat").FirstOrDefault();

                    // Confirming the map is at the root of the zip.
                    if (infoData == null)
                        return BadRequest();

                    // Upload the record.

                    BSFile file = new BSFile()
                    {
                        DownloadURL = $"download/{user.DiscordID}/{nextKey}",
                        Downloads = 0,
                        Key = nextKey,
                        Type = FileType.Map,
                        Name = body.Name,
                        Uploader = user.DiscordID,
                        HasPassword = body.Protected,
                        MaxDownloads = body.Max,
                        Password = body.Password,
                        UnlimitedDownloads = body.Unlimited,
                        Uploaded = DateTime.UtcNow
                    };
                    file = _fileService.Create(file);

                    // Save the file.

                    using Stream stream = System.IO.File.Create(userFolder + "/" + file.Key + ".zip");
                    await body.File.CopyToAsync(stream);

                    // Validation Test

                    /*using StreamReader infoReader = new StreamReader(infoData.Open());
                    var info = await infoReader.ReadToEndAsync();
                    JObject infoObject = JObject.Parse(info);

                    var valid = infoObject.IsValid(_schemaService.InfoSchema);
                    */

                    return Ok($"The map has been uploaded successfully. The key is {file.Key}");
                }
                catch
                {
                    return Ok("There was an error while uploading the map. Please contact Auros#0001 on Discord.");
                }
            }
            return NotFound();
        }

        public class FileUpload
        {
            public string Type { get; set; }
            public IFormFile File { get; set; }
            public string Name { get; set; }
            public bool Unlimited { get; set; } = true;
            public bool Protected { get; set; } = false;
            public string Password { get; set; } = "";
            public int Max { get; set; } = 0;
        }
    }
}
