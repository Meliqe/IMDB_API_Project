
using System.Data;
using Microsoft.Data.SqlClient;

namespace Imdb.Repositories;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
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
                cmd.CommandType = CommandType.StoredProcedure; //bunu kullan normal query göre farkı nedir login ve register sp si olsun
                cmd.Parameters.AddWithValue("@Isim", user.Name);
                cmd.Parameters.AddWithValue("@Soyisim", user.Surname);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Sifre", user.Password);
                cmd.Parameters.AddWithValue("@Telefon", user.Phone);
                
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    
    //Kullanıcı Giriş
    public bool KullaniciGiris(string email, string password)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("kisi_giris", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Sifre", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var mesaj = reader["Mesaj"].ToString();
                        var durum = Convert.ToInt32(reader["Durum"]);

                        Console.WriteLine($"Sonuç: {mesaj}, Durum: {durum}");
                        return durum == 1;
                    }
                }
            }
        }
        return false;
    }
}