
namespace VideoOverflow.Core;

public interface IResourceRepository
{
    public Task<IEnumerable<ResourceDTO>> GetAll();
    public Task<IEnumerable<ResourceDTO>> GetByTags();
    public Task<ResourceDetailsDTO?> Get(int id);
    public Task<ResourceDTO> Push(ResourceCreateDTO create);
    public Task<Status> Update(ResourceUpdateDTO update);

}