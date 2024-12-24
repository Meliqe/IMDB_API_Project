using Imdb.Repositories;
using Imdb.Services;
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
    
}