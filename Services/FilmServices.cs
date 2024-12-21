using Imdb.Models;
using Imdb.Repositories;

namespace Imdb.Services;

public class FilmServices
{
    private readonly FilmRepository _filmRepository;
    private readonly IConfiguration _configuration;

    public FilmServices(FilmRepository filmRepository, IConfiguration configuration)
    {
        _filmRepository = filmRepository;
        _configuration = configuration;
    }

    public List<Film> GetFilms()
    {
        try
        {
            var films = _filmRepository.GetAllFilms();
            return films;
        }
        catch (Exception ex)
        {
           Console.WriteLine(ex.Message);
           return new List<Film>(); //boş film listesi döner 
        }
    }
}