namespace VideoOverflow.Repository.Infrastructure.Repositories;

public class UserRepository
{
    private readonly VideoOverflowContext _context;

    public UserRepository(VideoOverflowContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<UserDetailsDTO>() ReadAll()
    {
        // add code 
    }
    
    public async Task<UserDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<UserDetailsDTO> Create(UserDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, TagUpdateDTO update)
    {
        // add code
    }
    
}