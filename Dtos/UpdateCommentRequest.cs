namespace Imdb.Dtos;

public class UpdateCommentRequest
{
    public Guid CommentId { get; set; }   
    public Guid UserId { get; set; }     
    public string Content { get; set; }  
}