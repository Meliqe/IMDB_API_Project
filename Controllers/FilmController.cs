using Imdb.Models;
using Imdb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;
[ApiController]
[Route("api/[controller]")]
public class FilmController:ControllerBase
{
    private readonly FilmServices _filmService;

    public FilmController(FilmServices filmService)
    {
        _filmService = filmService;
    }

    [HttpGet("allfilms")]
    public IActionResult GetFilms()
    {
        try
        {
            var films = _filmService.GetFilms();
            if (films.Count == 0)
            {
                return NotFound("Hiç film bulunamadı.");
            }
            return Ok(films); // 200 OK ve film listesi döndür.
        }
        catch (Exception ex)
        {
            return StatusCode(500,ex.Message);
        }
    }
    
    [HttpGet("allgenres")]
    public IActionResult GetGenres()
    {
        try
        {
            var genres = _filmService.GetGenres();
           
            if (genres.Count == 0)
            {
                return NotFound("Hiç film bulunamadı.");
            }
            return Ok(genres); // 200 OK ve film listesi döndür.
        }
        catch (Exception ex)
        {
            return StatusCode(500,ex.Message);
        }
    }

    [HttpGet("allactors")]
    public IActionResult GetActors()
    {
        var actors = _filmService.GetActors();
        if (actors.Count == 0)
        {
            return NotFound("Actor bilgisi gelmedi");
        }
        return Ok(actors);
    }

    [HttpGet("filmdetails/{id}")]
    public IActionResult GetFilmDetails(Guid id)
    {
        try
        {
            var(film,actor,genre) = _filmService.GetFilmById(id);
            var response = new
            {
                Film = film,
                Actor = actor,
                Genre = genre
            };
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,"Film detayları gelmedi");
        }
    }

    [HttpGet("oyuncudetails/{id}")]
    public IActionResult GetActorDetails(Guid id)
    {
        try
        {
               var actor = _filmService.GetActorById(id);
               return Ok(actor);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,"Oyuncu detayları gelmedi!");
        }
    }

    [HttpGet("filmsbycategoryname/{categoryName}")]
    public IActionResult GetFilmsByCategoryName(string categoryName)
    {
        try
        {
            var films = _filmService.GetFilmsByCategoryName(categoryName);
            return Ok(films);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,"İlgili kategoriye ait filmler gelmedi!");
        }
    }

    [Authorize(Roles = "user")]
    [HttpPost("addcomment")]
    public IActionResult AddComment([FromBody] Comment comment)
    {
        if (comment.UserId == Guid.Empty || comment.FilmId == Guid.Empty || string.IsNullOrEmpty(comment.Content))
        {
            return BadRequest("Geçersiz yorum bilgileri.");
        }
        try
        {
            _filmService.AddComment(comment);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine("yorum eklenemedi!!");
            return StatusCode(500,"yorum eklerken bir hata oluştu");
        }
    }

    [HttpGet("allcommentsbyfilmid/{filmid}")]
    public IActionResult GetCommentsByFilmId(Guid filmid)
    {
        try
        {
            var comments = _filmService.GetCommentsByFilmId(filmid);
            return Ok(comments);
        }
        catch (Exception e)
        {
            return StatusCode(500,"filme gelen yorumları görüntülerken hata!!");
        }
    }
}