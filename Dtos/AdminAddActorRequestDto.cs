namespace Imdb.Dtos;

public class AdminAddActorRequestDto
{
    public Guid FilmId { get; set; }
    public string ActorName { get; set; }
    public string PhotoPath { get; set; }
    public string Biography { get; set; }
    public DateTime BirthDate { get; set; }
}