using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExtModule.API.App.Interfaces;
using ExtModule.API.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace ExtModule.API.Infra.Repositories
{
    public class JwtTokenService : IJWTTokenService
    {


        public string GenerateToken(string username, string Jwtkey, string JwtIssuer, string JwtAudience)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
            
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwtkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public void SetTokenInsideCookie(string accessToken,string refreshToken,HttpContext context)
        {
            context.Response.Cookies.Append("accessToken", accessToken,
                
                new CookieOptions
                {
                    Expires= DateTimeOffset.UtcNow.AddMinutes(30),
                    HttpOnly= true,
                    IsEssential= true,
                    SameSite=SameSiteMode.None,
                });
            context.Response.Cookies.Append("refreshToken", refreshToken,

               new CookieOptions
               {
                   Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                   HttpOnly = true,
                   IsEssential = true,
                   SameSite = SameSiteMode.None,
               });
        }
    }
}
