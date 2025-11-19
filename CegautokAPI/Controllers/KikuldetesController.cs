using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KikuldetesController : ControllerBase
    {
        [HttpGet("Kikuldetes")]
        public IActionResult GetAllKikuldetesek()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<Kikuldete> kikuldesek = context.Kikuldetes.ToList();
                    return Ok(kikuldesek);
                }
                catch (Exception ex)
                {
                    List<Kikuldete> valasz = new List<Kikuldete>();
                    Kikuldete hiba = new Kikuldete()
                    {
                        Id = -1,
                        Celja = $"Hiba a betöltés során: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("KikuldetesById")]
        public IActionResult GetKikuldetesById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    Kikuldete valasz = context.Kikuldetes.FirstOrDefault(u => u.Id == id);
                    if (valasz != null)
                    {
                        return Ok(valasz);
                    }
                    else
                    {
                        Kikuldete hiba = new Kikuldete()
                        {
                            Id = -1,
                            Celja = "Nincs ilyen azonosítójú kiküldetés!"
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

        [HttpPost("NewKikuldetes")]
        public IActionResult NewKikuldete(Kikuldete kikuldetes)
        {
            using (var context = new FlottaContext())

                try
                {
                    context.Kikuldetes.Add(kikuldetes);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés!");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
                }
        }

        [HttpPut("ModifyKikuldetes")]
        public IActionResult ModifyUser(Kikuldete kikuldetes)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if (context.Kikuldetes.Select(u => u.Id).Contains(kikuldetes.Id))
                    {
                        context.Kikuldetes.Update(kikuldetes);
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

        [HttpDelete("DelKikuldetes")]
        public IActionResult DeleteKikuldetes(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {

                    if (context.Kikuldetes.Select(u => u.Id).Contains(id))
                    {
                        Kikuldete torlendo = new Kikuldete { Id = id };
                        context.Kikuldetes.Remove(torlendo);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen kiküldetés!");
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
