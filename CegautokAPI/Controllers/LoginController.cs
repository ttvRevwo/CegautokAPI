using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
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
                        return NotFound("Nincs ilyen felhasználó, vagy inaktív a fiók!");
                    return Ok("Sikeres bejelentkezés, küldöm a tokent");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a bejelentkezés során: {ex.Message}");
                }
            }
        }
    }
}