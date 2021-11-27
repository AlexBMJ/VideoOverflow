namespace VideoOverflow.Infrastructure.Entities;
public class Tag
{
    //Attributes
    public int Id { get; set; }
    public string Name { get; set; }

    //Relations
    public ICollection<TagSynonym> TagSynonyms { get; set; }
    public ICollection<Resource> Resources { get; set; }
}