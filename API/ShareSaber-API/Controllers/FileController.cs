using ShareSaber_API.Models;
using ShareSaber_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ShareSaber_API.Controllers
{
    [Route("api/files"), ApiController]
    public class FileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FileService _fileService;

        public FileController(UserService userService, FileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        [HttpGet("key/{key}", Name = "GetFileInfo")]
        public IActionResult GetFileData(string key)
        {
            BSFile file = _fileService.Get(key);
            if (file == null)
                return NotFound();
            return Ok(file);
        }

        [Authorize, HttpGet("uploaded")]
        public IActionResult GetFilesUploadedByUser()
        {
            User user = _userService.UserFromContext(HttpContext);
            return Ok(_fileService.GetFilesUploadedByUser(user));
        }
    }
}
