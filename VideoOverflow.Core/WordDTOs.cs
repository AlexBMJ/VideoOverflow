namespace VideoOverflow.Core;

public record WordDTO(int Id, int Hash, string String);

public record WordCreateDTO
{
    public int Hash { get; init; }
    public string String { get; init; }
}

public record WordUpdateDTO : WordCreateDTO
{
    public int Id { get; init; }
}