using Imdb.Dtos;
using Imdb.Models;
using Imdb.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;
[ApiController]
[Route("api/[controller]")]
public class AdminController:ControllerBase
{
    private AdminServices _adminServices;

    public AdminController(AdminServices adminServices)
    {
        _adminServices = adminServices;
    }

    [HttpPost("addfilm")]
    public IActionResult AddFilm([FromBody] Film film)
    {
        try
        {
            var f = _adminServices.AddFilm(film);
            return Ok(f);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "film eklenemedi");
        }
    }

    [HttpDelete("deletefilm/{id}")]
    public IActionResult DeleteFilm(Guid id)
    {
        try
        {
            _adminServices.DeleteFilm(id);
            return Ok(new { message = "film silindi" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "film silinemedi");
        }
    }

    [HttpPost("addactor")]
    public IActionResult AddActor([FromBody] AdminAddActorRequestDto adminAddActorRequestDto)
    {
        try
        {
            var actor = _adminServices.AddActor(adminAddActorRequestDto);
            return Ok(actor);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Oyuncu eklenemedi");
        }
    }

    [HttpGet("admingetfilmbyid/{id}")]
    public IActionResult GetAdmingetFilmbyid(Guid id)
    {
        try
        {
            var film = _adminServices.GetFilmById(id);
            return Ok(film);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "İlgili filmin detayları gelemedi");
        }
    }

    [HttpPut("updatefilmbyid/{id}")]
    public IActionResult UpdateFilmById(Guid id,[FromBody] Film film)
    {
        try
        {
            film.FilmId = id;
            var filmUpdated = _adminServices.UpdateFilmById(film);
            return Ok(filmUpdated);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Film bilgileri güncellenemedi");
        }
    }

    [HttpPut("updateactorbyid/{id}")]
    public IActionResult UpdateActorById(Guid id, [FromBody] Actor actor)
    {
        try
        {
            actor.Id = id;
            var actorUpdated = _adminServices.UpdateActorById(actor);
            return Ok(actorUpdated);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Oyuncu bilgisi güncellenemedi");
        }
    }
    
}