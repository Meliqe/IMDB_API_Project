using Imdb.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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
                                    FilmReleaseDate = reader["yayinlanma_tarihi"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["yayinlanma_tarihi"])
                                        : (DateTime?)null,
                                    FilmDuration = reader["film_suresi"] != DBNull.Value
                                        ? Convert.ToInt32(reader["film_suresi"])
                                        : (int?)null,
                                    PosterPath = reader["poster_url"]?.ToString()
                                };

                                films.Add(film);
                            }
                        }
                    }
                }
            return films;
        }
    }
}
