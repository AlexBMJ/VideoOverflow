namespace VideoOverflow.Core;

public interface ITagSynonymRepository
{
    
    public Task<IReadOnlyCollection<TagSynonymDTO>> GetAll();
    
    public Task<TagSynonymDTO?> GetTagSynByName(string tagSyn);
    public Task<TagSynonymDTO?> Get(int id);
    public Task<TagSynonymDTO> Push(TagSynonymCreateDTO create);
    public Task<Status> Update(TagSynonymUpdateDTO update);
}