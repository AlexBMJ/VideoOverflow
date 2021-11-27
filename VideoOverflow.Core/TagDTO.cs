namespace VideoOverflow.Core;

public record TagDTO(int Id, string Name);

public record TagCreateDTO
{
    public string Name { get; init; }
    public ICollection<string>? TagSynonyms { get; init; }
}

public record TagUpdateDTO : TagCreateDTO
{
    public int Id { get; init; }
}

    
