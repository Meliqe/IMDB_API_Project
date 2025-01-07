﻿using Imdb.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using Imdb.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Imdb.Repositories
{
    public class FilmRepository
    {
        private readonly string _connectionString;

        public FilmRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Tüm filmleri getir
        public List<Film> GetAllFilms()
        {
            var films = new List<Film>();
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("GetFilmBilgileri", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var film = new Film
                                {
                                    FilmId = (Guid)reader["film_id"],
                                    FilmName = reader["film_adi"]?.ToString(),
                                    FilmDescription = reader["film_aciklamasi"]?.ToString(),
                                    Genres = reader["turler"]?.ToString()?.Split(',')
                                        .Select(t => new Genre
                                        {
                                            GenreName = t.Trim()
                                        }).ToList(),
                                    Actors = reader["oyuncular"]?.ToString()?.Split(',')
                                        .Select(t => new Actor
                                        {
                                            ActorName = t.Trim()
                                        }).ToList(),
                                    FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                                    FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                                    RateAvg = reader["score"]!= DBNull.Value
                                        ? Convert.ToSingle(reader["score"]) 
                                        : 0f,
                                    PosterPath = reader["poster_url"] as byte[] != null 
                                        ? Convert.ToBase64String((byte[])reader["poster_url"]) 
                                        : null
                                };
 
                                //Console.WriteLine("film idleri:" +film.FilmId);
                                films.Add(film);
                            }
                        }   
                    }
                }
            return films;
        }

        public List<Genre> GetAllGenre()
        {
            var genres = new List<Genre>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetTurBilgileri", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var genre = new Genre
                            { 
                                Id = Convert.ToInt32(reader["tur_id"]),
                               GenreName = reader["tur_adi"]?.ToString(),
                            };
                            genres.Add(genre);
                        }
                    }
                }
            }
            return genres;
        }

        public List<Actor> GetAllActors()
        {
            var actors = new List<Actor>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetOyuncuBilgileri", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var actor = new Actor()
                            {
                                Id = (Guid)reader["oyuncu_id"],
                                ActorName = reader["oyuncu_adi"]?.ToString(),
                                PhotoPath = reader["oyuncu_fotografi"] as byte[] != null 
                                    ? Convert.ToBase64String((byte[])reader["oyuncu_fotografi"]) 
                                    : null,
                                Biography = reader["oyuncu_biyografi"]?.ToString(),
                                BirthDate = Convert.ToDateTime(reader["oyuncu_dogum_tarihi"])
                            };
                            actors.Add(actor);
                        }
                    }
                }
            }
            return actors;
        }

        public (Film Film, List<Actor>Actors, List<Genre> Genres) GetFilmById(Guid filmId) //tuple özelliği
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetFilmByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var filmIDParameter = new SqlParameter
                    {
                        ParameterName = "@FilmID", //procedure gitcek
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = filmId
                    };
                    cmd.Parameters.Add(filmIDParameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Film film = null;
                        while (reader.Read())
                        {
                            film = new Film
                            {
                                FilmId = (Guid)reader["film_id"],
                                FilmName = reader["film_adi"]?.ToString(),
                                FilmDescription = reader["film_aciklamasi"]?.ToString(),
                                FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                                FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                                PosterPath = reader["poster_url"] as byte[] != null
                                ? Convert.ToBase64String((byte[])reader["poster_url"])
                                :null,
                                RateAvg = reader["OrtalamaPuan"] != DBNull.Value ? Convert.ToSingle(reader["OrtalamaPuan"]) : 0.0f
                            };
                        }

                        var actors=new List<Actor>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                actors.Add(new Actor
                                {
                                    ActorName = reader["oyuncu_adi"]?.ToString(),
                                    PhotoPath = reader["oyuncu_fotografi"] as byte[] != null
                                    ? Convert.ToBase64String((byte[])reader["oyuncu_fotografi"])
                                    : null,
                                });
                            }
                        }

                        var genres = new List<Genre>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                genres.Add(new Genre
                                {
                                    GenreName = reader["tur_adi"]?.ToString()
                                });
                            }
                        }
                        return (film, actors, genres);
                    }
                }
            }
        }

        public Actor GetActorById(Guid actorId)
        {
            Actor actor = null;
            using (var conn=new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetOyuncuByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var actorIdParameter = new SqlParameter
                    {
                        ParameterName = "@OyuncuID", //procedurun beklediği parametre
                        //ismi aynı olmak zorunda
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = actorId
                    };
                    cmd.Parameters.Add(actorIdParameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            actor = new Actor()
                            {
                                Id = (Guid)reader["oyuncu_id"],
                                ActorName = reader["oyuncu_adi"]?.ToString(),
                                Biography = reader["oyuncu_biyografi"]?.ToString(),
                                BirthDate = Convert.ToDateTime(reader["oyuncu_dogum_tarihi"]),
                                PhotoPath = reader["oyuncu_fotografi"] as byte[] != null
                                    ? Convert.ToBase64String((byte[])reader["oyuncu_fotografi"])
                                    :null,
                            };
                        }   
                        return actor;
                    }
                }
            }
        }

        public List<Film> GetFilmsByCategoryName(string categoryName)
        {
            var films = new List<Film>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetFilmsByCategoryName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var categoryNameParameter = new SqlParameter
                    {
                        ParameterName = "@CategoryName",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = categoryName
                    };
                    cmd.Parameters.Add(categoryNameParameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var film = new Film()
                            {
                                FilmId = (Guid)reader["film_id"],
                                FilmName = reader["film_adi"]?.ToString(),
                                FilmDescription = reader["film_aciklamasi"]?.ToString(),
                                FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                                FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                                PosterPath = reader["poster_url"] as byte[] != null
                                    ? Convert.ToBase64String((byte[])reader["poster_url"])
                                    : null,
                            };
                            films.Add(film);
                        }
                    }
                }
            }
            return films;
        }
        
        public Comment AddComment(Comment comment)
        {
            Comment cmt = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("yorum_ekle", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@KullaniciID",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = comment.UserId
                    });
                    
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@FilmID",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = comment.FilmId
                    });

                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Yorum",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = comment.Content
                    });

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmt = new Comment
                            {
                                CommentId = (Guid)reader["yorum_id"],
                                Content = reader["yorum"].ToString(),
                                PublishedDate = Convert.ToDateTime(reader["yorum_yayinlanma_tarihi"]),
                                UserId = (Guid)reader["userid"],
                                FilmId = (Guid)reader["film_id"],
                            };
                        }
                        return cmt;
                    }
                }
            }
        }

        public List<Comment> GetCommentsByFilmId(Guid filmId)
        {
            var comments = new List<Comment>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("GetCommentsByFilmID", conn)) 
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
                        while (reader.Read())
                        {
                            comments.Add(new Comment
                            {
                                CommentId = (Guid)reader["yorum_id"], 
                                Content = reader["yorum"].ToString(), 
                                PublishedDate = Convert.ToDateTime(reader["yorum_yayinlanma_tarihi"]),
                                UserId = (Guid)reader["userid"], 
                                FilmId = (Guid)reader["film_id"]
                            });
                        }
                    }
                }
            }
            return comments; 
        }

        public void AddFilmToList(FilmListRequestDto filmListRequestDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd=new SqlCommand("FilmEkle",conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@kullanici_id",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = filmListRequestDto.UserId
                    });
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@film_id",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = filmListRequestDto.FilmId
                    });
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("film listeye eklendi");
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Film listeye eklenirken hata: " + ex.Message);
                    }
                }
            }
        }
        
        public void RemoveFilmFromList(FilmListRequestDto filmListRequestDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("FilmSil", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@kullanici_id", filmListRequestDto.UserId);
                    cmd.Parameters.AddWithValue("@film_id", filmListRequestDto.FilmId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("film listeden kaldırıldı");
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Film listeden kaldırılamadı: " + ex.Message);
                    }
                }
            }
        }

    }
}
