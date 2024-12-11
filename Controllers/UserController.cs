
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
            user.Password = HashHelper.GetHash(user.Password);

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
            if (storedUser == null)
            {
                return Unauthorized("E-posta bulunamadı.");
            }
            Console.WriteLine($"Kullanıcıdan gelen parola: {user.Password}");
            Console.WriteLine($"Veritabanından alınan hash: {storedUser.Password}");

            var isPasswordValid = HashHelper.VerifyHash(user.Password, storedUser.Password);
            Console.WriteLine($"Parola doğrulama sonucu: {isPasswordValid}");
            if (!isPasswordValid)
            {
                return Unauthorized("E-posta veya şifre hatalı.");
            }
            return Ok("Giriş başarılı.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}