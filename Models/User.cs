using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User
{
    public Guid Id { get; set; }
    
    [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Sadece harfler kullanılabilir...")]
    public string? Name { get; set; }
    
    [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Sadece harfler kullanılabilir...")] 
    public string? Surname { get; set; }
    
    [EmailAddress(ErrorMessage = "Geçersiz mail adresi...")]
    public string Email { get; set; }
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Şifre en az 6 karakter, büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    public string Password { get; set; }
    
    [RegularExpression(@"^05\d{9}$", ErrorMessage = "Yanış formatta numara...")]
    public string? Phone { get; set; }
    public DateTime CreateTime { get; set; }
    
}
