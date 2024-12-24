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
                                    FilmId = Guid.NewGuid(),
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
                                    PosterPath = reader["poster_url"]?.ToString()
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
                                Id = Guid.Parse(reader["oyuncu_id"].ToString()),
                                ActorName = reader["oyuncu_adi"]?.ToString(),
                                PhotoPath = reader["oyuncu_fotografi"]?.ToString(),
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
    }
}
