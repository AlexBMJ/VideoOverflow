namespace VideoOverflow.Core.DTOs;

public record CategoryDTO(int Id, string Name);

public record CategoryCreateDTO()
{
    public string Name { get; init; }
}

public record CategoryUpdateDTO : CategoryCreateDTO
{
    public int Id { get; init; }
}