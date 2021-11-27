using System.ComponentModel.DataAnnotations;

namespace VideoOverflow.Infrastructure.Entities;
public class Resource
{
    //Attributes
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string MaterialType { get; set; }
    [Url]
    public string Site_url { get; set; }
    public string Site_title { get; set; }
    public string? Author { get; set; }
    public string Content_source { get; set; }
    public int? LixNumber { get; set; }
    public int? SkillLevel { get; set; }
    public string Langauge { get; set; }
    
    //Relations
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    
}