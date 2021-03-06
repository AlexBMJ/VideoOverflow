namespace VideoOverflow.Infrastructure.Entities;

/// <summary>
/// The tag entity
/// </summary>
public class Tag
{
    //Attributes
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }

    //Relations
    public ICollection<TagSynonym>? TagSynonyms { get; set; }
    public ICollection<Resource>? Resources { get; set; }
}