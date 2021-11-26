namespace VideoOverflow.Repository.Infrastructure.Repositories;

public class TagRepository
{
    private readonly VideoOverflowContext _context;

    public TagRepository(VideoOverflowContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<TagDetailsDTO>() ReadAll()
    {
        // add code 
    }
    
    public async Task<TagDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<TagDetailsDTO> Create(TagDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, TagUpdateDTO update)
    {
        // add code
    }

}