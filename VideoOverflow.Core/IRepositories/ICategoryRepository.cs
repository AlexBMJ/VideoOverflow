
namespace VideoOverflow.Core;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<CategoryDTO>> GetAll();
    Task<CategoryDTO> Get(int id);
    Task<CategoryDTO> Push(CategoryCreateDTO resource);
    Task<Status> Update(CategoryUpdateDTO resource);

}