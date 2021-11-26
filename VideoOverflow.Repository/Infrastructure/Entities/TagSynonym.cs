namespace VideoOverflow.Repository.Infrastructure.Entities;
public class TagSynonym
{
    //Attributes
    public int Id { get; set; }
    public string Name { get; set; }

    //Relations
    public IReadOnlyCollection<Tag> Tags { get; set; }
}