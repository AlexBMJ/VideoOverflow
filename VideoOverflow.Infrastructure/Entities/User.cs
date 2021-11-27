namespace VideoOverflow.Infrastructure.Entities;

public class User
{
    //Attributes
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Relations
    public ICollection<Comment>? Comments { get; set; }
}