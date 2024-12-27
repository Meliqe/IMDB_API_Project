using Imdb.Models;
using Imdb.Repositories;
using Imdb.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
}