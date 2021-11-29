using System.ComponentModel.DataAnnotations;

namespace VideoOverflow.Infrastructure.Entities;
public class Resource
{
    //Attributes
    public int Id { get; set; }
    public DateTime? Created { get; set; }
    public ResourceType MaterialType { get; set; }
    [Url]
    public string SiteUrl { get; set; }
    public string SiteTitle { get; set; }
    public string Author { get; set; }
    public string ContentSource { get; set; }
    public int LixNumber { get; set; }
    public int SkillLevel { get; set; }
    public string? Language { get; set; }
    
    //Relations
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    
}