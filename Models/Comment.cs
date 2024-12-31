namespace Imdb.Models;

public class Comment
{
    public Guid CommentId { get; set; }
    public string Content { get; set; }
    public DateTime PublishedDate { get; set; }
    //navigation properties
    public Guid UserId { get; set; } //foreign key
    public Guid FilmId { get; set; } //foreign key
}