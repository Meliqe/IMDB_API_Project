namespace Imdb.Models;

public class Actor
{
    public Guid Id { get; set; }
    public string? ActorName { get; set; }
    public string? PhotoPath { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
}