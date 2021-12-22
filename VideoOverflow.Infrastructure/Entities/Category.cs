namespace VideoOverflow.Infrastructure.Entities;

/// <summary>
/// The category entity
/// </summary>
public class Category
{
    // Attributes
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }
    
    // Relation
    public ICollection<Resource>? Resources { get; set; }
}