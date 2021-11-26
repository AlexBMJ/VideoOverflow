namespace VideoOverflow.Core; 
public record CategoryDTO(int Id, string Name);

public record CategoryCreateDTO()
{
    public string Name;
}

public record CategoryUpdateDTO : CategoryCreateDTO {
    public int Id { get; init; }
}