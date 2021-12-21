using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the tagSynonym repository. This ensures all the crud methods are implemented
/// </summary>
public interface ITagSynonymRepository
{
    
    public Task<IReadOnlyCollection<TagSynonymDTO>> GetAll();
    
    public Task<TagSynonymDTO?> GetTagSynByName(string tagSyn);
    
    public Task<TagSynonymDTO?> Get(int id);
    
    public Task<TagSynonymDTO> Push(TagSynonymCreateDTO create);
    public Task<Status> Update(TagSynonymUpdateDTO update);
}