﻿using Imdb.Models;
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
    
}