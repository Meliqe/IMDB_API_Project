using Imdb.Models;
using Imdb.Services;
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
}