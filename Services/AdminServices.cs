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
}