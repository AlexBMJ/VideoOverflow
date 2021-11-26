namespace VideoOverflow.Repository.Infrastructure.Repositories;

public class CommentRepository
{
    private readonly VideoOverflowContext _context;

    public CommentRepository(VideoOverflowContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<CommentDetailsDTO>() ReadAll()
    {
        // add code 
    }
    
    public async Task<CommentDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<CommentDetailsDTO> Create(CommentDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, CommentUpdateDTO update)
    {
        // add code
    }
    
}