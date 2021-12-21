using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the category repository. This ensures all the crud methods are implemented
/// </summary>
public interface ICategoryRepository
{

    Task<IReadOnlyCollection<CategoryDTO>> GetAll();
    
    Task<CategoryDTO?> Get(int id);
    
    Task<CategoryDTO> Push(CategoryCreateDTO category);

    Task<Status> Update(CategoryUpdateDTO category);

}