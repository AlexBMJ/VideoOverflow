namespace VideoOverflow.Infrastructure.Entities;

/// <summary>
/// The comment entity
/// </summary>
public class Comment
{
    //Attributes
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public int AttachedToResource { get; set; }
    
    [MaxLength(300)]
    public string Content { get; set; }
}