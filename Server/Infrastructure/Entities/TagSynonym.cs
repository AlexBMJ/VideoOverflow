namespace VideoOverflow.Server.Infrastructure.Entities;
public class TagSynonym
{

    public int Id { get; set; }
    
    public string Name { get; set; }

    public IReadOnlyCollection<Tag> Tags { get; set; }
}