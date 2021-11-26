namespace VideoOverflow.Infrastructure.Entities;

public class Category
{
    // Attributes
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Relation
    public ICollection<Resource>? Resources { get; set; }
}