namespace VideoOverflow.Core.DTOs;

public record CommentDTO(int Id, int CreatedBy, int AttachedToResource, string Content);

public record CommentCreateDTO
{
    public int CreatedBy { get; init; }
    public string Content { get; init; }
    public int AttachedToResource { get; set; }
}

public record CommentUpdateDTO : CommentCreateDTO
{
    public int Id { get; init; }   
}

    
