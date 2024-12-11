using BCrypt.Net;

namespace Imdb.Helpers;

public class HashHelper
{
    public static string GetHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    //doğrulama işlemi yani girilen parola ile daha önceden hashlenere saklanmış parola aynı mı ?
    public static bool VerifyHash(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

}