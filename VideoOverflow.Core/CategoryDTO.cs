namespace VideoOverflow.Core; 
public record CategoryDTO(int Id);
public record CategoryDetailsDTO();

public record CategoryCreateDTO() {
    
}

public record eCategoryUpdateDTO : CategoryCreateDTO {
    public int Id { get; init; }
}