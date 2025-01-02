using Imdb.Helpers;
using Imdb.Repositories;


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
        //user.Role="admin";
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
        Console.WriteLine(storedUser.Role);
        var isPasswordValid = HashHelper.VerifyHash(user.Password, storedUser.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }
        return JwtHelper.GenerateJwtToken(storedUser.Email,storedUser.Role,storedUser.Id, _configuration);
    }

    public User GetUserById(Guid userId)
    {
        try
        {
            var user = _userRepository.GetUserById(userId);
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}