
using AutoMapper;
using ExtModule.API.App.Interfaces;
using ExtModule.API.Application.Interfaces;
using ExtModule.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using static ExtModule.API.Core.ERP.F8API;

namespace ExtModule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IJWTTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IERPRepository _ERPRepository;
        private readonly IMapper _mapper;

        public AuthController(IJWTTokenService tokenService, IConfiguration configuration,IERPRepository eRPRepository,IMapper mapper)
        {
            _tokenService = tokenService;
            _configuration = configuration;
            _ERPRepository = eRPRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Validates the user details passed
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost("LoginBySession")]
        public async Task<IActionResult> LoginBySession([FromBody] UserLoginDto userLogin)
        {
            // Replace this with actual user verification
            string baseFocusAPIUrl = _configuration.GetValue<string>("AppSettings:FocusAPI");
            //var obj= (await _ERPRepository.validateFocusSessionId(userLogin.SessionId,userLogin.CompanyCode, baseFocusAPIUrl));
            var obj = new LoginRes
            {
                Data = null,
                 Message = "",
                Result = 1,
                Url = ""
            };
            if (obj.Result==1)
            {                
                    var token = _tokenService.GenerateToken(userLogin.SessionId, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    _tokenService.SetTokenInsideCookie(token, refreshToken, HttpContext);                 

                return Ok(new { message = "Logged in successfully" });
                //return Ok(new { Token=token , RefreshToken=refreshToken });

            }
         

            return Unauthorized();
        }

    }
}
