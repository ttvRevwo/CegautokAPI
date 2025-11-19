using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CegautokAPI.Models;
using CegautokAPI.Controllers;
using System.Linq.Expressions;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("User")]
        public IActionResult GetAllUsers()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<User> felhasznalok = context.Users.ToList();
                    return Ok(felhasznalok);
                }
                catch (Exception ex)
                {
                    List<User> valasz = new List<User>();
                    User hiba = new User()
                    {
                        Id = -1,
                        Name = $"Hiba a betöltés során: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("UserById")]
        public IActionResult GetUserById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    User valasz = context.Users.FirstOrDefault(u => u.Id == id);
                    if (valasz != null)
                    {
                        return Ok(valasz);
                    }
                    else
                    {
                        User hiba = new User()
                        {
                            Id = -1,
                            Name = "Nincs ilyen azonosítójú felhasználó!"
                        };
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba az adatok betöltése során: {ex.Message}");
                }
            }
        }

        [HttpPost("NewUser")]
        public IActionResult NewUser(User user)
        {
            using (var context = new FlottaContext())

                try
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés!");
                }
                catch (Exception ex)
                { 
                    return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
                }
        }

        [HttpPut("ModifyUser")]
        public IActionResult ModifyUser(User user)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if (context.Users.Select(u => u.Id).Contains(user.Id))
                    {
                        context.Users.Update(user);
                        context.SaveChanges();
                    };
                    return Ok("Sikeres módosítás.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás közben: {ex.Message}");
                }
            }
        }

        [HttpDelete("DelUser")]
        public IActionResult DeleteUser(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {

                    if (context.Users.Select(u => u.Id).Contains(id))
                    {
                        User torlendo = new User { Id = id };
                        context.Users.Remove(torlendo);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen felhasználó!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben: {ex.Message}");
                }
            }
        }
    } 
}