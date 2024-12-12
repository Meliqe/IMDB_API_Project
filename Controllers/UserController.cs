
using Imdb.Helpers;
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
            /*var hashedPassword = BCrypt.Net.BCrypt.HashPassword("deneme123456");
            var isMatch = BCrypt.Net.BCrypt.Verify("deneme123456", hashedPassword);
            Console.WriteLine(isMatch);*/
            
            var storedUser = _userRepository.KullaniciBilgiGetir(user.Email);
            var isPasswordValid = HashHelper.VerifyHash(user.Password, storedUser.Password);
            Console.WriteLine($"Giriş Şifresi: {user.Password}");
            Console.WriteLine($"Veritabanındaki Hash: {storedUser.Password}");
            Console.WriteLine(isPasswordValid);
            if (isPasswordValid)
            {
                return Ok("Giriş başarılı.");
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