﻿using System.ComponentModel.DataAnnotations;

namespace VideoOverflow.Core; 
public record ResourceDTO(int Id,
    ResourceType MaterialType,
    string SiteUrl,
    string SiteTitle,
    string? Author,
    string Language,
    ICollection<string> Tags,
    ICollection<string> Categories,
    ICollection<string>? Comments);

public record ResourceDetailsDTO()
{
    public int Id { get; init; }
    public DateTime Created { get; init; }
    public ResourceType MaterialType { get; init; }
    [Url]
    public string SiteUrl { get; init; }
    public string SiteTitle { get; init; }
    public string? Author { get; init; }
    public string ContentSource { get; init; }
    public int? LixNumber { get; init; }
    public int? SkillLevel { get; init; }
    public string Langauge { get; init; }
    
    //Relations
    public ICollection<string> Tags { get; init; }
    public ICollection<string> Categories { get; init; }
    public ICollection<string>? Comments { get; init; }
};

public record ResourceCreateDTO {
    
    public DateTime Created { get; init; }
    public ResourceType MaterialType { get; init; }
    [Url]
    public string SiteUrl { get; init; }
    public string SiteTitle { get; init; }
    public string? Author { get; init; }
    public string ContentSource { get; init; }
    public int? LixNumber { get; init; }
    public int? SkillLevel { get; init; }
    public string Language { get; init; }
    
    //Relations
    public ICollection<string> Tags { get; init; }
    public ICollection<string> Categories { get; init; }
    public ICollection<string>? Comments { get; init; }
}

public record ResourceUpdateDTO : ResourceCreateDTO {
    public int Id { get; init; }
}