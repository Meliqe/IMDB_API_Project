using Imdb.Dtos;
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
           throw;
        }
    }

    public List<Genre> GetGenres()
    {
        try
        {
            var genres = _filmRepository.GetAllGenre();
            return genres;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public List<Actor> GetActors()
    {
        try
        {
            var actors = _filmRepository.GetAllActors();
            return actors;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public (Film film, List<Actor> actors , List<Genre> genres) GetFilmById(Guid filmId)
    {
        try
        {
            (Film Film, List<Actor> Actors, List<Genre> Genres) filmDetails = _filmRepository.GetFilmById(filmId);
           return filmDetails;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        
    }

    public Actor GetActorById(Guid actorId)
    {
        try
        {
            var actor = _filmRepository.GetActorById(actorId);
            return actor;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<Film> GetFilmsByCategoryName(string categoryName)
    {
        try
        {
            var films = _filmRepository.GetFilmsByCategoryName(categoryName);
            return films;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void AddComment(Comment comment)
    {
        try
        {
            _filmRepository.AddComment(comment);
            Console.WriteLine("yorum başarıyla eklendi");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //mevcut hatayı üst katmana fırlat
            throw;
        }
    }

    public List<Comment> GetCommentsByFilmId(Guid filmId)
    {
        try
        {
            var comments = _filmRepository.GetCommentsByFilmId(filmId);
            return comments;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void AddFilmToList(FilmListRequestDto filmListRequestDto)
    {
        try
        {
             _filmRepository.AddFilmToList(filmListRequestDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string RemoveFilmFromList(FilmListRequestDto filmListRequestDto)
    {
        try
        {
            var message= _filmRepository.RemoveFilmFromList(filmListRequestDto);
            return message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}