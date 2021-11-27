namespace VideoOverflow.Core;

public record UserDTO(int Id, string Name, ICollection<string>? Comments);

public record UserCreateDTO
{
    public string Name { get; init; }
    public ICollection<string>? Comments { get; init; }
}

public record UserUpdateDTO : UserCreateDTO
{
    public int Id { get; init; }
}