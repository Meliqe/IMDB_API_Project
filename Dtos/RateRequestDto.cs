namespace Imdb.Dtos;

public class RateRequestDto
{
    public Guid UserId { get; set; }
    public Guid FilmId { get; set; }
    public int Score { get; set; }
}