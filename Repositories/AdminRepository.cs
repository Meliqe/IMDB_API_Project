using System.Data;
using Imdb.Dtos;
using Imdb.Models;
using Microsoft.Data.SqlClient;

namespace Imdb.Repositories;

public class AdminRepository
{
    private readonly string _connectionString;
    public AdminRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public Film AddFilm(Film film)
    {
        Film f = null;
        using (var conn= new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd= new SqlCommand("AdminFilmEkle",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@film_adi",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = film.FilmName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@film_aciklamasi",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = film.FilmDescription
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@yayinlanma_tarihi",
                    SqlDbType = SqlDbType.DateTime,
                    Value = film.FilmReleaseDate
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@film_suresi",
                    SqlDbType = SqlDbType.Int,
                    Value = film.FilmDuration
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@poster_url_base64",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = film.PosterPath
                });
                // Genres listesini virgülle ayrılmış string formatına dönüştür
                var genresString = string.Join(",", film.Genres.Select(g => g.GenreName));
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName =  "@turler",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = genresString
                });
                using (var reader= cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        f = new Film
                        {
                            FilmId = (Guid)reader["film_id"],
                            FilmName = reader["film_adi"].ToString(),
                            FilmDescription = reader["film_aciklamasi"].ToString(),
                            FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                            FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                            PosterPath = reader["poster_url"] as byte[] != null 
                                ? Convert.ToBase64String((byte[])reader["poster_url"]) 
                                : null,
                            Genres = reader["turler"] != DBNull.Value 
                                ? reader["turler"].ToString()
                                    .Split(',')
                                    .Select(g => new Genre { GenreName = g.Trim() }) 
                                    .ToList()
                                : new List<Genre>()
                        };
                    }
                    return f;
                }
            }
        }
    }

    public void DeleteFilm(Guid filmId)
    {
        using (var conn= new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd= new SqlCommand("AdminFilmSil",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FilmID",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = filmId
                });
                try
                {
                    // ExecuteNonQuery dönen etkilenen satır sayısını kontrol ediyoruz
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        // Hiçbir satır etkilenmediyse hata fırlat
                        throw new Exception("Silinmek istenen film bulunamadı.");
                    }

                    Console.WriteLine("film silindi");
                }
                catch (SqlException ex)
                {
                    throw new Exception("Film silinirken hata: " + ex.Message);
                }
            }
        }
    }

    public Actor AddActor(AdminAddActorRequestDto adminAddActorRequestDto)
    {
        Actor actor = null;
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("AdminFilmeOyuncuEkle",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@film_id",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = adminAddActorRequestDto.FilmId
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@oyuncu_adi",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = adminAddActorRequestDto.ActorName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@oyuncu_fotografi",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = adminAddActorRequestDto.PhotoPath
                });cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@oyuncu_biyografi",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = adminAddActorRequestDto.Biography
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@oyuncu_dogum_tarihi",
                    SqlDbType = SqlDbType.DateTime,
                    Value = adminAddActorRequestDto.BirthDate
                });

                using (var reader= cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actor = new Actor
                        {
                            Id = (Guid)reader["oyuncu_id"],
                            ActorName = (string)reader["oyuncu_adi"],
                            Biography = reader["oyuncu_biyografi"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["oyuncu_dogum_tarihi"]),
                            PhotoPath = reader["oyuncu_fotografi"] as byte[] != null 
                                ? Convert.ToBase64String((byte[])reader["oyuncu_fotografi"]) 
                                : null
                        };
                    }
                    return actor;
                }
            }
        }
    }

    public Film GetFilmById(Guid filmId)
    {
        Film film = null;
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("AdminGetFilmById",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FilmID",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = filmId
                });

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        film = new Film
                        {
                            FilmId = (Guid)reader["film_id"],
                            FilmName = reader["film_adi"].ToString(),
                            FilmDescription = reader["film_aciklamasi"].ToString(),
                            FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                            FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                            PosterPath = reader["poster_url"] as byte[] != null 
                                ? Convert.ToBase64String((byte[])reader["poster_url"]) 
                                : null,
                            Actors = new List<Actor>(), // Listeler başlatılıyor
                            Genres = new List<Genre>()
                        };
                    }
                    
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            film.Actors.Add(new Actor
                            {
                                Id = (Guid)reader["oyuncu_id"],
                                ActorName = reader["oyuncu_adi"].ToString(),
                                PhotoPath = reader["oyuncu_fotografi"] as byte[] != null 
                                    ? Convert.ToBase64String((byte[])reader["oyuncu_fotografi"]) 
                                    : null,
                            });
                        } 
                    }
                    
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            film.Genres.Add(new Genre { GenreName = reader["tur_adi"].ToString() });
                        }
                        
                    }
                    return film;
                }
            }
        }
    }
}