using System.ComponentModel.DataAnnotations;

namespace VideoOverflow.Infrastructure.Entities;

/// <summary>
/// The resource entity
/// </summary>
public class Resource
{
    //Attributes
    public int Id { get; set; }
    public DateTime? Created { get; set; }
    public ResourceType MaterialType { get; set; }
    [Url]
    [MaxLength(500)]
    public string SiteUrl { get; set; }
    
    [MaxLength(500)]
    public string SiteTitle { get; set; }
    
    [MaxLength(200)]
    public string Author { get; set; }
    
    [MaxLength(200)]
    public string ContentSource { get; set; }
    public int LixNumber { get; set; }
    public int SkillLevel { get; set; }
    
    [MaxLength(100)]
    public string? Language { get; set; }
    
    //Relations
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    
}
