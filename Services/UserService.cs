using System.Linq.Expressions;
using Imdb.Helpers;
using Imdb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Imdb.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserService(UserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public bool Register(User user)
    {
        user.Id = Guid.NewGuid();        
        user.CreateTime = DateTime.Now;
        var result = _userRepository.KullaniciKayit(user);
        return result;
    }

    public string Login(User user)
    {
        var storedUser = _userRepository.KullaniciBilgiGetir(user.Email);
        if (storedUser == null)
        {
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
        }
        
        var isPasswordValid = HashHelper.VerifyHash(user.Password, storedUser.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }
        return JwtHelper.GenerateJwtToken(user.Email, _configuration);
    }
}