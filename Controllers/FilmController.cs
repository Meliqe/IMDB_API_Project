using Imdb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;
[ApiController]
public class FilmController:ControllerBase
{
    private readonly FilmRepository _filmRepository;

    public FilmController(FilmRepository filmRepository)
    {
        _filmRepository = filmRepository;
    }

    [HttpGet("api/films")]
    public IActionResult GetFilms()
    {
        try
        {
            var films = _filmRepository.GetAllFilms();
            if (films == null || films.Count == 0)
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
    
}