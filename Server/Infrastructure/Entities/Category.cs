namespace VideoOverflow.Server.Infrastructure.Entities;

public class Category
{
    // Attributes
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Relation
    public IReadOnlyCollection<Resource> Resources { get; set; }
}