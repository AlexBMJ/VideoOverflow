namespace VideoOverflow.Infrastructure;

public class CategoryRepository
{

    private readonly IVideoOverflowContext _context;

    public CategoryRepository(IVideoOverflowContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<CategoryDetailsDTO>> ReadAll()
    {
        // add code 
    }
    
    public async Task<CategoryDetailsDTO?> ReadAll(int id)
    {
        // add code 
    }

    public async Task<CategoryDetailsDTO> Create(CategoryDTO create)
    {
        // add code
    }

    public async Task<Status> Update(int id, CategoryUpdateDTO update)
    {
        // add code
    }

}