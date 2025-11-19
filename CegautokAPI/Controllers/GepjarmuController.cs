using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GepjarmuController : ControllerBase
    {
        [HttpGet("Gepjarmu")]
        public IActionResult GetAllGepjarmus()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<Gepjarmu> gepjarmuvek = context.Gepjarmus.ToList();
                    return Ok(gepjarmuvek);
                }
                catch (Exception ex)
                {
                    List<Gepjarmu> valasz = new List<Gepjarmu>();
                    Gepjarmu hiba = new Gepjarmu()
                    {
                        Id = -1,
                        Marka = $"Hiba a betöltés során: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("GepjarmuById")]
        public IActionResult GetGepjarmuById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    Gepjarmu valasz = context.Gepjarmus.FirstOrDefault(u => u.Id == id);
                    if (valasz != null)
                    {
                        return Ok(valasz);
                    }
                    else
                    {
                        Gepjarmu hiba = new Gepjarmu()
                        {
                            Id = -1,
                            Marka = "Nincs ilyen azonosítójú gépjármű!"
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

        [HttpPost("NewGepjarmu")]
        public IActionResult NewGepjarmu(Gepjarmu gepjarmu)
        {
            using (var context = new FlottaContext())

                try
                {
                    context.Gepjarmus.Add(gepjarmu);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés!");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
                }
        }

        [HttpPut("ModifyGepjarmu")]
        public IActionResult ModifyGepjarmu(Gepjarmu gepjarmu)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if (context.Gepjarmus.Select(u => u.Id).Contains(gepjarmu.Id))
                    {
                        context.Gepjarmus.Update(gepjarmu);
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

        [HttpDelete("DelGepjarmu")]
        public IActionResult DeleteGepjarmu(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {

                    if (context.Gepjarmus.Select(u => u.Id).Contains(id))
                    {
                        Gepjarmu torlendo = new Gepjarmu { Id = id };
                        context.Gepjarmus.Remove(torlendo);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen gépjármű!");
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
