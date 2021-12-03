namespace VideoOverflow.Core.DTOs;

public record TagSynonymDTO(int Id, string Name);

public record TagSynonymCreateDTO
{
    public string Name { get; init; }
}

public record TagSynonymUpdateDTO : TagSynonymCreateDTO
{
    public int Id { get; init; }
}