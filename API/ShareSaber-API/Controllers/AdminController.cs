using ShareSaber_API.Models;
using ShareSaber_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using ShareSaber_API.Types;
using System;

namespace ShareSaber_API.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize, HttpPost("updateuser")]
        public IActionResult DownloadFile([FromBody] QueryUserUpdate body)
        {
            if (body.Target == null)
                return BadRequest();

            var user = _userService.UserFromContext(HttpContext);
            if (user.Role != ShareSaberRole.Admin && user.Role != ShareSaberRole.Owner)
                return BadRequest();

            if (body.Target == user.DiscordID)
                return BadRequest();

            var prole = (ShareSaberRole)body.Role;
            if (prole == ShareSaberRole.Owner)
                return BadRequest();

            var targetUser = _userService.Get(body.Target);
            if (targetUser == null)
                return NotFound();

            if (targetUser.Role == ShareSaberRole.Owner)
                return BadRequest();

            targetUser.Role = prole;

            _userService.Update(targetUser.DiscordID, targetUser);

            return NoContent();
        }

        public class QueryUserUpdate
        {
            public string Target { get; set; }
            public int Role { get; set; }
        }
    }
}
