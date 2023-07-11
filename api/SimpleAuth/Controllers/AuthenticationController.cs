using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleAuth.Models;

namespace SimpleAuth.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpGet("simple-login")]
        public ActionResult<string> GetToken(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest();
        
            return Authenticate(new UserDto() { Username = username, Password = password });
        }

        [HttpPost("simple-login")]
        public ActionResult<string> PostToken(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest();

            return Authenticate(new UserDto() { Username = username, Password = password });
        }

        private ActionResult<string> Authenticate(UserDto user)
        {
            var headerNeeded = _configuration.GetSection("HeadersNeeded").Get<string[]>();
            if (headerNeeded is not null && headerNeeded.Any())
            {
                if (headerNeeded.Select(header => Request.Headers[header].ToList()).Any(headers => !headers.Any()))
                {
                    return BadRequest("Missing headers");
                }
            }
            
            return Ok(CreateToken(new UserDto {
                Username = user.Username,
                Password = user.Password
            }));
        }
        
        private string CreateToken(UserDto user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var toto = _configuration.GetSection("AppSettings:Token").Value!;
            SymmetricSecurityKey key = new(
                Encoding.UTF8.GetBytes(toto));

            SigningCredentials cred = new(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}