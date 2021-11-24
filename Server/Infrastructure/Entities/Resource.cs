namespace VideoOverflow.Server.Infrastructure.Entities;
public class Resource
{
    
    //Attributed

    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string MaterialType { get; set; }
    public string Site_url { get; set; }
    public string Site_title { get; set; }
    public string Creator { get; set; }
    public string Content_source { get; set; }
    public int LixNumber { get; set; }
    public int SkillLevel { get; set; }
    public string Langauge { get; set; }
    
    //Relations
    public IReadOnlyCollection<Tag> Tags { get; set; }
    public IReadOnlyCollection<Category> Categories { get; set; }
    public IReadOnlyCollection<Comment> Comments { get; set; }
    
}