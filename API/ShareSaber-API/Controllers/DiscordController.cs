using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShareSaber_API.Services;
using ShareSaber_API.Models;
using ShareSaber_API.Types;


namespace ShareSaber_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        private readonly ILogger<DiscordController> _logger;

        private readonly UserService _userService;
        private readonly DiscordService _discordService;
        private readonly JWTService _jwtService;

        public DiscordController(ILogger<DiscordController> logger, UserService userService, DiscordService discordService, JWTService jwtService)
        {
            _logger = logger;

            _userService = userService;
            _discordService = discordService;
            _jwtService = jwtService;
        }

        [HttpGet("auth")]
        public IActionResult Authenticate() =>
            Redirect($"https://discordapp.com/api/oauth2/authorize?response_type=code&client_id={_discordService.ID}&scope=identify&redirect_uri={_discordService.RedirectURL}");

        [HttpGet("authorize")]
        public async Task<IActionResult> Authed([FromQuery(Name = "code")] string code)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                // Load all the discord related profile information
                string accessCode = await _discordService.SendDiscordOAuthRequestViaAuthCode(code);
                DiscordUser discord = await _discordService.GetDiscordUserProfile(accessCode);

                User user = _userService.Get(discord.ID);
                if (user == null)
                {
                    // If the user doesn't exist, create it and add it to the database.
                    user = new User
                    {
                        DiscordID = discord.ID,
                        Banned = false,
                        Role = ShareSaberRole.None
                    };
                    user = _userService.Create(user);
                    _logger.LogInformation($"User {discord.Username}#{discord.Discriminator} ({user.DiscordID}) has created an account from {ip}.");
                }
                //_logger.LogInformation($"User {discord.Username}#{discord.Discriminator} ({user.DiscordID}) has logged in from {ip}.");

                // Generate the JWT token to ship off to the user
                string token = _jwtService.GenerateUserToken(user);
                return Ok(new { token, user, discord });
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
    }
}
