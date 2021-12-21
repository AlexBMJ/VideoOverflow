using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the tag repository. This ensures all the crud methods are implemented
/// </summary>
public interface ITagRepository
{
    
    public Task<IReadOnlyCollection<TagDTO>> GetAll();
    
    public Task<TagDTO?> GetTagByName(string tagName);
    
    public Task<IReadOnlyCollection<TagDTO>> GetTagByNameAndSynonym(string name);
    
    public Task<TagDTO?> Get(int tagId);
    
    public Task<TagDTO> Push(TagCreateDTO tag);
    public Task<Status> Update(TagUpdateDTO update);
}