using Imdb.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;

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
                                    PosterPath = reader["poster_url"] as byte[] != null 
                                        ? Convert.ToBase64String((byte[])reader["poster_url"]) 
                                        : null
                                };

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
                                FilmDuration = Convert.ToInt32(reader["film_suresi"]),
                                FilmReleaseDate = Convert.ToDateTime(reader["yayinlanma_tarihi"]),
                                PosterPath = reader["poster_url"] as byte[] != null
                                ? Convert.ToBase64String((byte[])reader["poster_url"])
                                :null,
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
    }
}
