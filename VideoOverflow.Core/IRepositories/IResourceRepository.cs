using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;
public interface IResourceRepository
{
    public Task<IEnumerable<ResourceDTO>> GetAll();
    public Task<IEnumerable<ResourceDTO>> GetResources(int category, string query, IEnumerable<TagDTO> tags, int count, int skip);
    public Task<Option<ResourceDetailsDTO>> Get(int id);
    public Task<ResourceDTO> Push(ResourceCreateDTO create);
    public Task<Status> Update(ResourceUpdateDTO update);

    public Task<Status> Delete(int resourceId);
}