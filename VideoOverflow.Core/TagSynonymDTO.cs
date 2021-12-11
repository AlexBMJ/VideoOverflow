namespace VideoOverflow.Core;

public record TagSynonymDTO(int Id, string Name, ICollection<string>? Tags);

public record TagSynonymCreateDTO
{
    public string Name { get; init; }
    public ICollection<string>? Tags { get; init; }
}

public record TagSynonymUpdateDTO : TagSynonymCreateDTO
{
    public int Id { get; init; }
}