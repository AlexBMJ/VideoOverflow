namespace VideoOverflow.Infrastructure.Entities;

/// <summary>
/// The tagSynonym entity
/// </summary>
public class TagSynonym
{
    //Attributes
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }

    //Relations
    public ICollection<Tag>? Tags { get; set; }
}