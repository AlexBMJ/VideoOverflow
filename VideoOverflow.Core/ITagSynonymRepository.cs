namespace VideoOverflow.Core;

public interface ITagSynonymRepository
{
    
    public Task<IEnumerable<TagSynonymDTO>> GetAll();
    public Task<TagSynonymDTO?> Get(int id);
    public Task<TagSynonymDTO> Push(TagSynonymCreateDTO create);
    public Task<Status> Update(TagSynonymUpdateDTO update);
}