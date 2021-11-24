namespace VideoOverflow.Server.Infrastructure.Entities;
public class Tag
{
  
    public int Id { get; set; }
    
    public string Name { get; set; }

    //Relations
    public IReadOnlyCollection<TagSynonym> TagSynonyms { get; set; }
    public IReadOnlyCollection<Resource> Resources { get; set; }
}