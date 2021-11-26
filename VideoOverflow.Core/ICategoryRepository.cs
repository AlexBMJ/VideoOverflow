
namespace VideoOverflow.Core;

public interface ICategoryRepository
{
    Task<IEnumerable<ResourceDetailsDTO>> ReadAll();
    Task<ResourceDetailsDTO?> ReadAll(int id);
    Task<ResourceDetailsDTO> Create(ResourceDTO create);
    Task<Status> Update(int id, ResourceUpdateDTO update);

}