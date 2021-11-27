namespace VideoOverflow.Infrastructure.Entities;
public class TagSynonym
{
    //Attributes
    public int Id { get; set; }
    public string Name { get; set; }

    //Relations
    public ICollection<Tag> Tags { get; set; }
}