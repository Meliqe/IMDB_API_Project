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
    public string Password { get; set; }
    
    [RegularExpression(@"^05\d{8}$", ErrorMessage = "Yanış formatta numara...")]
    public string? Phone { get; set; }
    public DateTime CreateTime { get; set; }
    
}
