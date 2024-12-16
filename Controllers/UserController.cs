
using System.Linq.Expressions;
using Imdb.Helpers;
using Imdb.Repositories;
using Imdb.Services;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;
// jwt geldikten sonra bir şey eklemem gerekbilir. atribute gibi
//"AllowedHosts": "*",  bu kısma da bak 
[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("kayit")]
    public IActionResult Register([FromBody]User user)
    {
        try
        {
            var result = _userService.Register(user);
            if(result)
                return Ok();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata: {ex.Message}");
        }
    }
    
    [HttpPost("giris")]
    public IActionResult Login([FromBody] User user)
    {
        try
        {
            var token = _userService.Login(user);
            return Ok(new {Token = token});
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata: {ex.Message}");
        }
    }
}