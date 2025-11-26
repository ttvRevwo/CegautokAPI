using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("{id}/Hasznalat")]
        public IActionResult GetHasznalatById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<JarmuHasznalatDTO> valasz = context.Kikuldottjarmus.Include(k => k.Kikuldetes).Include(k => k.GepJarmu).Where(j => j.GepJarmuId == id).Select(j => new JarmuHasznalatDTO
                    {
                        Id = id,
                        Rendszam = j.GepJarmu.Rendszam,
                        Kezdes = j.Kikuldetes.Kezdes,
                        Befejezes = j.Kikuldetes.Befejezes
                    }).OrderBy(j => j.Kezdes).ToList();
                    return Ok(valasz);
                }
                catch (Exception ex)
                {
                    List<JarmuHasznalatDTO> valasz = new List<JarmuHasznalatDTO>() { new()
                    {
                        Id = -1,
                        Rendszam = "hiba" } };
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("Sofor")]
        public IActionResult GetSofor()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<SoforDTO> valasz = context.Kikuldottjarmus
                        .Include(j => j.GepJarmu)
                        .Include(j => j.SoforNavigation)
                        .GroupBy(j => new { rsz = j.GepJarmu.Rendszam, so = j.SoforNavigation.Name })
                        .Select(elem => new SoforDTO()
                        {
                            Rendszam = elem.Key.rsz,
                            SoforNev = elem.Key.so,
                            Darab = elem.Count()
                        }).ToList();
                    return Ok(valasz);
                }
                catch (Exception ex)
                {
                    List<SoforDTO> valasz = new List<SoforDTO>() { new()
                    {
                        Rendszam = "hiba",
                        SoforNev = ex.Message
                    } };
                    return BadRequest(valasz);
                }
            }
        }
        // KikuldetesController, adott ID-jű kiküldetésen ki volt a sofor
    }
}
