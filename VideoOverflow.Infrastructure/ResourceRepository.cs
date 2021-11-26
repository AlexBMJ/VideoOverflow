namespace VideoOverflow.Infrastructure;

public class ResourceRepository : IResourceRepository
{
    private readonly IVideoOverflowContext _context;

    public ResourceRepository(IVideoOverflowContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<ResourceDetailsDTO>> ReadAll()
    {
        // add code
    }
    
    public async Task<ResourceDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<ResourceDetailsDTO> Create(ResourceDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, ResourceUpdateDTO update)
    {
        // add code
    }
}