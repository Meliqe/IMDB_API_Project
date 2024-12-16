
using System.Data;
using Imdb.Helpers;
using Microsoft.Data.SqlClient;

namespace Imdb.Repositories;

public class UserRepository
{
    private readonly string _connectionString;
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    //Fonksiyonlar

    //Kullanıcı Kayıt
    public bool KullaniciKayit(User user)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("kisi_ekle", conn))
            {
                user.Password=HashHelper.GetHash(user.Password);
                Console.WriteLine($"Kaydedilecek Şifre Hash: {user.Password}");
                Console.WriteLine($"Hash Uzunluğu: {user.Password.Length}");
                cmd.CommandType = CommandType.StoredProcedure; 
                cmd.Parameters.AddWithValue("@Isim", user.Name); //add fonksiyonu sqlparameterdan 
                cmd.Parameters.AddWithValue("@Soyisim", user.Surname);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Sifre", user.Password);
                cmd.Parameters.AddWithValue("@Telefon", user.Phone);
                
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    //Kullanici bilgilerini Getiren fonksiyon
    public User KullaniciBilgiGetir(string email)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("kullanici_bilgi_getir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    {
                        var user = new User
                        {
                            Password = reader.GetString(reader.GetOrdinal("sifre")) // Hashlenmiş parola
                        };
                        //Console.WriteLine($"Veritabanından alınan parola hash: {user.Password}");
                        return user;
                    }
                }
            }
        }
        return null;
    }
    
    
    
    
    
}