namespace VideoOverflow.Core; 
public record ResourceDTO(int Id);
public record ResourceDetailsDTO();

public record ResourceCreateDTO() {
    
}

public record ResourceUpdateDTO : ResourceCreateDTO {
    public int Id { get; init; }
}