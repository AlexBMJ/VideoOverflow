using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

public interface ITagRepository
{
    
    public Task<IReadOnlyCollection<TagDTO>> GetAll();

    public Task<TagDTO?> Get(int tagId);

    public Task<TagDTO> Push(TagCreateDTO tag);

    public Task<Status> Update(TagUpdateDTO update);
}