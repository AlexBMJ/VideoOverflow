namespace VideoOverflow.Core.DTOs;

public record TagDTO(int Id, string Name, ICollection<string>? TagSynonyms);

public record TagCreateDTO
{
    public string Name { get; init; }
    public ICollection<string>? TagSynonyms { get; init; }
}

public record TagUpdateDTO : TagCreateDTO
{
    public int Id { get; init; }
}