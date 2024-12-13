
using Imdb.Helpers;
using Imdb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Imdb.Controller;
// jwt geldikten sonra bir şey eklemem gerekbilir. atribute gibi
//"AllowedHosts": "*",  bu kısma da bak 
[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    private readonly UserRepository _userRepository;
    private IConfiguration _configuraiton;

    public UserController(IConfiguration configuration)
    {
        _configuraiton = configuration;
        _userRepository = new UserRepository(_configuraiton.GetConnectionString("DefaultConnection"));
    }

    [HttpPost("kayit")]
    public IActionResult Register([FromBody]User user)
    {
        try
        {
            user.Id = Guid.NewGuid();        
            user.CreateTime = DateTime.Now;
            var result = _userRepository.KullaniciKayit(user);
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
            var storedUser = _userRepository.KullaniciBilgiGetir(user.Email);
            var isPasswordValid = HashHelper.VerifyHash(user.Password.Trim(), storedUser.Password.Trim());
            var email = user.Email;
            if (isPasswordValid)
            {
                return Ok(new { Token = JwtHelper.GenerateJwtToken(email, _configuraiton) });
            }
            return Unauthorized("E-posta veya şifre hatalı.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}