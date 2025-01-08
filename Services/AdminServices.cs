using Imdb.Dtos;
using Imdb.Models;
using Imdb.Repositories;

namespace Imdb.Services;

public class AdminServices
{
    private AdminRepository _adminRepository;
    private readonly IConfiguration _configuration;

    public AdminServices(AdminRepository adminRepository, IConfiguration configuration)
    {
        _adminRepository = adminRepository;
        _configuration = configuration;
    }

    public Film AddFilm(Film film)
    {
        try
        {
            if (!string.IsNullOrEmpty(film.PosterPath) && film.PosterPath.StartsWith("data:image"))
            {
                film.PosterPath = film.PosterPath.Substring(film.PosterPath.IndexOf(",") + 1); 
            }
            var f = _adminRepository.AddFilm(film);
            return f;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DeleteFilm(Guid filmId)
    {
        try
        {
            _adminRepository.DeleteFilm(filmId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Actor AddActor(AdminAddActorRequestDto adminAddActorRequestDto)
    {
        try
        {
            if (!string.IsNullOrEmpty(adminAddActorRequestDto.PhotoPath) && adminAddActorRequestDto.PhotoPath.StartsWith("data:image"))
            {
                adminAddActorRequestDto.PhotoPath = adminAddActorRequestDto.PhotoPath.Substring(adminAddActorRequestDto.PhotoPath.IndexOf(",") + 1); 
            }
            var actor = _adminRepository.AddActor(adminAddActorRequestDto);
            return actor;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Film GetFilmById(Guid filmId)
    {
        try
        {
            var film = _adminRepository.GetFilmById(filmId);
            return film;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Film UpdateFilmById(Film film)
    {
        try
        {
            if (!string.IsNullOrEmpty(film.PosterPath) && film.PosterPath.StartsWith("data:image"))
            {
                film.PosterPath = film.PosterPath.Substring(film.PosterPath.IndexOf(",") + 1); 
            }
            var filmUpdated = _adminRepository.UpdateFilmById(film);
            return filmUpdated;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}