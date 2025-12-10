using System.Security.Claims;
using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Jwtsettings _jwtSettings;

        public LoginController(Jwtsettings jwtsettings)
        {
            _jwtSettings = jwtsettings;
        }

        [HttpGet("GetSalt")]
        public IActionResult GetSalt(string loginName)
        {
            using (var context = new FlottaContext())
            {
                //Get the salt and send it back or what upon html login request which is just a form rn
                try
                {
                    User user = context.Users.FirstOrDefault(u => u.Name == loginName);
                    if (user != null)
                    {
                        return Ok(user.Salt);
                    }
                    else
                    {
                        return NotFound("Nincs ilyen felhasználónév!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a kérés teljesítése során: {ex.Message}");
                }

            }
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    string doubleHash = Program.CreateSHA256(loginDTO.SentHash);
                    User user = context.Users.FirstOrDefault(u =>
                    u.LoginName == loginDTO.LoginName &&
                    u.Hash == doubleHash &&
                    u.Active);
                    if (user == null)
                    {
                        return NotFound("Nincs ilyen felhasználó, vagy inaktív a fiók!");
                    }
                    var claims = new[]
{
                        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirityMinutes),
                        signingCredentials: creds);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a bejelentkezés során: {ex.Message}");
                }
            }
        }
    }
}