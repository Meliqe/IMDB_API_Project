namespace Imdb.Models;

public class Rate
{
    public Guid RateId { get; set; }
    public int Score { get; set; } 
    public DateTime RatingDate { get; set; } 
    public Guid UserId { get; set; } 
    public Guid FilmId { get; set; } 
}