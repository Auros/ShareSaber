using System;
using System.Text;
using ShareSaber_API.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ShareSaber_API.Services
{
    public class JWTService
    {
        public string Key { get; }
        public string Issuer { get; }

        public JWTService(IJWT settings)
        {
            Key = settings.Key;
            Issuer = settings.Issuer;
        }

        public string GenerateUserToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.DiscordID),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }
    }
}
