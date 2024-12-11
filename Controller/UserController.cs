
using Imdb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    private readonly UserRepository _userRepository;

    public UserController(IConfiguration configuration)
    {
        _userRepository = new UserRepository(configuration.GetConnectionString("DefaultConnection"));
    }

    [HttpPost("kayit")]
    public IActionResult Register([FromBody]User user)
    {
        try
        {
            user.Id = Guid.NewGuid();        
            user.CreateTime = DateTime.Now;
            bool result = _userRepository.KullaniciKayit(user);
            if (result)
                return Ok("Kayıt başarılı."); 
            return BadRequest("Kayıt başarısız.");
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
            bool result = _userRepository.KullaniciGiris(user.Email, user.Password);

            if (result)
                return Ok("Giriş başarılı."); 

            return Unauthorized("E-posta veya şifre hatalı."); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata: {ex.Message}");
        }
    }
}