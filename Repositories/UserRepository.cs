
using System.Data;
using Imdb.Dtos;
using Imdb.Helpers;
using Imdb.Models;
using Microsoft.Data.SqlClient;

namespace Imdb.Repositories;

public class UserRepository
{
    private readonly string _connectionString;
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    

    //Kullanıcı Kayıt
    public bool KullaniciKayit(User user)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("kisi_ekle", conn))
            {
                user.Password=HashHelper.GetHash(user.Password);
                //Console.WriteLine($"Kaydedilecek Şifre Hash: {user.Password}");
                //Console.WriteLine($"Hash Uzunluğu: {user.Password.Length}");
                cmd.CommandType = CommandType.StoredProcedure; 
                cmd.Parameters.AddWithValue("@Isim", user.Name); //add fonksiyonu sqlparameterdan 
                cmd.Parameters.AddWithValue("@Soyisim", user.Surname);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Sifre", user.Password);
                cmd.Parameters.AddWithValue("@Telefon", user.Phone);
                if (!string.IsNullOrEmpty(user.Role)) //null değilse
                {
                    cmd.Parameters.AddWithValue("@Rol", user.Role);
                }
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
                            Id = (Guid)reader["id"],
                            Email = reader["email"].ToString(),
                            Password = reader.GetString(reader.GetOrdinal("sifre")), 
                            Role=reader.GetString(reader.GetOrdinal("rol")),
                        };
                        return user;
                    }
                }
            }
        }
        return null;
    }
    public User GetUserById(Guid userid)
    {
        User user = null;
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("GetKullaniciByID",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UserId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = userid
                });
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User
                        {
                            Id = (Guid)reader["id"],
                            Name = reader["isim"].ToString(),
                            Surname = reader["soyisim"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["sifre"].ToString(),
                            Phone = reader["telefon"].ToString(),
                            CreateTime = Convert.ToDateTime(reader["kayit_tarihi"]),
                            Role =reader["rol"].ToString(),
                        };
                    }
                    return user;
                }
            }
        }
    }

    public User KullaniciBilgiGuncelle(User user)
{
    using (var conn = new SqlConnection(_connectionString))
    {
        conn.Open();
        using (var cmd = new SqlCommand("kullaniciBilgiGuncelle", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@KullaniciID",
                SqlDbType = SqlDbType.UniqueIdentifier,
                Value = user.Id
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@KullaniciIsim",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(user.Name) ? DBNull.Value : user.Name
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@KullaniciSoyisim",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(user.Surname) ? DBNull.Value : user.Surname
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@KullaniciTelefon",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(user.Phone) ? DBNull.Value : user.Phone
            });

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read()) // sadece tek satır okuyacağı için
                {
                    return new User
                    {
                        Id = (Guid)reader["id"],
                        Name = reader["isim"].ToString(),
                        Surname = reader["soyisim"].ToString(),
                        Email = reader["email"].ToString(),
                        Password = reader["sifre"].ToString(),
                        Phone = reader["telefon"] != DBNull.Value ? reader["telefon"].ToString() : null,
                        CreateTime = Convert.ToDateTime(reader["kayit_tarihi"]),
                        Role = reader["rol"] != DBNull.Value ? reader["rol"].ToString() : null
                    };
                }
            }
        }
    }
    return null;
}

    public List<CommentsByUserDto> GetCommentsByUserId(Guid userId)
    {
        var comments = new List<CommentsByUserDto>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("GetCommentsByUserID", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@KullaniciID",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = userId
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var commentDto = new CommentsByUserDto
                        {
                            CommentId = (Guid)reader["yorum_id"],
                            Content = reader["yorum"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["yorum_yayinlanma_tarihi"]),
                            FilmId = (Guid)reader["film_id"],
                            FilmName = reader["film_adi"].ToString()
                        };
                       comments.Add(commentDto); 
                    }
                }
            }
        }
        return comments;
    }

    public CommentsByUserDto UpdateUserComment(Guid commentId, Guid userId, string content)
    {
        CommentsByUserDto comment = null;
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("EditComment",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@CommentId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = commentId
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UserId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = userId
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@Comment",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = content
                });

                using (var reader =cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comment = new CommentsByUserDto
                        {
                            CommentId = (Guid)reader["yorum_id"],
                            Content = reader["yorum"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["yorum_yayinlanma_tarihi"]),
                            FilmId = (Guid)reader["film_id"],
                            FilmName = reader["film_adi"].ToString()
                        };
                    }
                    return comment;
                }
            }
        }
    }


    public Rate AddOrUpdateRate(RateRequestDto rateRequestDto)
    {
        Rate rate = null;
        using (var conn= new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd=new SqlCommand("AddOrUpdatePuan", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UserId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = rateRequestDto.UserId
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FilmId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = rateRequestDto.FilmId
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@Puan",
                    SqlDbType = SqlDbType.Int,
                    Value = rateRequestDto.Score
                });

                using (var reader =cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        rate = new Rate
                        {
                            RateId = (Guid)reader["puan_id"],
                            Score = Convert.ToInt32(reader["puan"]),
                            RatingDate = Convert.ToDateTime(reader["puanlama_tarihi"]),
                            UserId = (Guid)reader["userid"],
                            FilmId = (Guid)reader["film_id"],
                            RateAvg = reader["OrtalamaPuan"] != DBNull.Value ? Convert.ToSingle(reader["OrtalamaPuan"]) : 0.0f
                        };
                    }
                    return rate;
                }
            }
        }
    }

    public List<Film> UserList(Guid userId)
    {
        List<Film> userlist = new List<Film>();
        using (var conn= new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd= new SqlCommand("GetUserWatchlist",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@kullanici_id",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = userId
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var film = new Film
                        {
                            FilmId = (Guid)reader["film_id"],
                            FilmName = reader["film_adi"].ToString(),
                            PosterPath = reader["poster_url"] as byte[] != null
                                ? Convert.ToBase64String((byte[])reader["poster_url"])
                                : null
                        };
                        userlist.Add(film);
                    }
                    return userlist;
                }
            }
        }
    }
}