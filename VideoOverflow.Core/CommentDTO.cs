namespace VideoOverflow.Core;

public record CommentDTO(int Id, string Content);

public record CommentCreateDTO
{
    public string Content { get; init; }
}

public record CommentUpdateDTO : CommentCreateDTO
{
    public int Id { get; init; }   
}

    
