using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExtModule.API.App.Interfaces
{
    public interface IJWTTokenService
    {
        public string GenerateToken(string username, string Jwtkey, string JwtIssuer, string JwtAudience);
        public string GenerateRefreshToken();
        public void SetTokenInsideCookie(string accessToken, string refreshToken, HttpContext context);
    }
}
