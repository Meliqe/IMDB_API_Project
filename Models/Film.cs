namespace Imdb.Models;

public class Film
{
    public Guid FilmId { get; set; }
    public string? FilmName { get; set; }
    public string? FilmDescription { get; set; }
    public DateTime? FilmReleaseDate { get; set; }
    public int? FilmDuration { get; set; }
    public string? PosterPath { get; set; }
    public List<Genre>? Genres { get; set; }
    public List<Actor>? Actors { get; set; }
    
    public float? RateAvg { get; set; }
}