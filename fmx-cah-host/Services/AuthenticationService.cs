using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fmx_cah_host.Services
{
    // TODO: Add Oauth for Discord?

    // Provides the authentication service for users
    public class AuthenticationService
    {
        private JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private SigningCredentials _signingCredentials = new SigningCredentials(Startup.JwtSigningKey, SecurityAlgorithms.HmacSha256);

        public string CreateJwtToken(string userId, string userName)
        {
            var claimPrincipals = BuildClaimsPrincipal(userId, userName);
            var token = new JwtSecurityToken(Startup.JwtIssuer, Startup.JwtAudience, claimPrincipals.Claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: _signingCredentials);
            return _tokenHandler.WriteToken(token);
        }


        private ClaimsPrincipal BuildClaimsPrincipal(string userId, string userName, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim("id", userId)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, Startup.JwtAuthScheme));
        }
    }
}
