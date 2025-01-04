
using System.Linq.Expressions;
using Imdb.Dtos;
using Imdb.Helpers;
using Imdb.Models;
using Imdb.Repositories;
using Imdb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//
namespace Imdb.Controller;
// jwt geldikten sonra bir şey eklemem gerekbilir. atribute gibi
//"AllowedHosts": "*",  bu kısma da bak 
[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase //Basecontroller diye kendi sınıfım oalcak tğm controllerımalrımı ordan üretcem
{
    //basemodel ekle
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("kayit")]
    public IActionResult Register([FromBody] User user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userService.Register(user);
            if (result)
                return Ok(new { success = true, message = "Kayıt başarılı!" });

            return BadRequest(new { success = false, message = "Kullanıcı kaydı başarısız oldu." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Sunucu hatası: {ex.Message}"
            });
        }
    }

    
    [HttpPost("giris")]
    public IActionResult Login([FromBody] User user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            var token = _userService.Login(user);
            return Ok(new {Token = token});
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Sunucu hatası: {ex.Message}"
            });
        }
    }

    [Authorize(Roles = "user")]
    [HttpGet("userdetails/{id}")]
    //routerdaki id string kabul ediliyor
    public IActionResult GetUserDetails(Guid id)
    {
        try
        {
            var user = _userService.GetUserById(id);
            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,"kullanıcı bilgileri gelmedi!!");
        }
    }
    
    [Authorize(Roles = "user")]
    [HttpPatch("updateuser")]
    public IActionResult UpdateUserInfo([FromBody] User user)
    {
        try
        {
            Console.WriteLine($"Gelen Veri: {user.Name}, {user.Surname}, {user.Phone}");
            var updateuser=_userService.KullaniciBilgiGuncelle(user);
            return Ok(updateuser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "kullanıcı bilgileri güncellenemedi!!");
        }
    }

    [Authorize(Roles = "user")]
    [HttpGet("usercomments/{id}")]
    public IActionResult GetCommentsByUserId(Guid id)
    {
        try
        {
            var response = _userService.GetCommnetsByUserId(id);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Kullanıcının yaptığı yorumlar gelmedi");
        }
    }

    [Authorize(Roles = "user")]
    [HttpPatch("updatecomment")]
    public IActionResult UpdateComment([FromBody] UpdateCommentRequest request)
    {
        try
        {
            var cmt=_userService.UpdateUserComment(request.CommentId, request.UserId, request.Content);
            return Ok(cmt);
        }
        catch (Exception e)
        {
            return StatusCode(500, "yorum güncellenemedi!!");
        }
    }
}