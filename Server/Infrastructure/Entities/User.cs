namespace VideoOverflow.Server.Infrastructure.Entities;

public class User
{
   
    public int Id { get; set; }

    public string Name { get; set; }
    
    // Relations
    public IReadOnlyCollection<Comment> Comments { get; set; }
}