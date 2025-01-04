namespace Imdb.Dtos;

public class CommentsByUserDto
{
    public Guid CommentId { get; set; }
    public string Content { get; set; }
    public DateTime PublishedDate { get; set; }
    public Guid FilmId { get; set; }
    public string FilmName { get; set; }
}