using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

namespace Imdb.Helpers;

public class JwtHelper
{
    //token üretecek metot,Bu metot, doğrulama işlemi değil, token üretimi için yazılmıştır.
    public static string GenerateJwtToken(string email, string role,IConfiguration configuration)
    {
        // 1. Gerekli ayarları alacağız
        // 2. Kullanıcı bilgileri için claim'ler oluşturacağız
        // 3. SecretKey ile token'ı imzalayacağız
        // 4. Token'ı oluşturup döndüreceğiz
        
        //jwt ayarlarını çekelim
        var jwtSettings= configuration.GetSection("JwtSettings");
        var issuer = jwtSettings.GetSection("Issuer").Value;
        var audience = jwtSettings.GetSection("Audience").Value;
        var expiration = int.Parse(jwtSettings.GetSection("ExpirationMinutes").Value);
        Env.Load();
        var secretKey = Env.GetString("JWT_SECRET_KEY");
        
        //claim oluşturuyoruz . claim kullanıcı bilgilerii temsil ediyor Kullanıcının kim olduğunu veya ne yetkilere sahip olduğunu bilmek istiyorsak, bu bilgileri token içine claim olarak ekleriz.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email), //sub tokenın kime ait olduğunu belirtir
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Token'ı eşsiz yapmak için
            new Claim(ClaimTypes.Role,role)
        };
 
        //token imzalama adımına geldik bunun için kendi secret keyimiz ile imzalayacağız
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // secret keyi byte haline getir
        //bu sınıfda gelen byte'ı kullnarak kriptogrofik key oluşturur
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // bu kod key ile HMAC-SHA256 algoritmasını kullanarak token'ın imzalanacağını belirten bir "imza bilgisi" (SigningCredentials) oluşturur.
        // //Header + Payload → Secret Key ile imzalanır → Signature elde edilir.

        //tokenın yapısı ise Header.Payload.Signature
        

        // Token oluşturma
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiration),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token); //tokenı string hale çevirip öyle yazıyoruz.
    }
}