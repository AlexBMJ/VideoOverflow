namespace VideoOverflow.Core;

public record CommentDTO(int Id, int CreatedBy, string Content);

public record CommentCreateDTO
{
    public int CreatedBy { get; init; }
    public string Content { get; init; }
}

public record CommentUpdateDTO : CommentCreateDTO
{
    public int Id { get; init; }   
}

    
