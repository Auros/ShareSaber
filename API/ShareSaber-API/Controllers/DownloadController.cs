using ShareSaber_API.Models;
using ShareSaber_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace ShareSaber_API.Controllers
{
    [Route("[controller]"), ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly FileService _fileService;

        public DownloadController(UserService userService, FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{user:length(18)}/{key}", Name = "DownloadFile")]
        public IActionResult DownloadFile(string user, string key, [FromQuery] string password = "")
        {
            BSFile file = _fileService.Get(key);
            if (file == null || file.Uploader != user) // This was intentional
                return NotFound();

            if (!file.UnlimitedDownloads && file.Downloads >= file.MaxDownloads)
                return BadRequest();

            if (file.HasPassword)
            {
                if (string.IsNullOrEmpty(password))
                    return BadRequest();
                if (password != file.Password)
                    return BadRequest();
            }

            file.Downloads++;
            _fileService.Update(key, file);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            var userFolder = Path.Combine(path, file.Uploader);
            
            Stream stream = System.IO.File.OpenRead(userFolder + "/" + file.Key + ".zip");

            return File(stream, "application/zip");

        }
    }
}
