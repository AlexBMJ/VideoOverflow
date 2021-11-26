namespace VideoOverflow.Repository.Infrastructure.Repositories;

public class TagSynonymRepository
{
    private readonly VideoOverflowContext _context;

    public TagSynonymRepository(VideoOverflowContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<TagSynonymDetailsDTO>() ReadAll()
    {
        // add code 
    }
    
    public async Task<TagSynonymDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<TagSynonymDetailsDTO> Create(TagSynonymDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, TagSynonymUpdateDTO update)
    {
        // add code
    }

}