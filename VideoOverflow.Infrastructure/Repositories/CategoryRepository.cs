namespace VideoOverflow.Infrastructure.repositories;

public class CategoryRepository : ICategoryRepository
{

    private readonly IVideoOverflowContext _context;

    public CategoryRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CategoryDTO>> GetAll()
    {
        return (await _context.Categories.Select(category => new CategoryDTO(category.Id, category.Name))
            .ToListAsync()).AsReadOnly();
    }

    public async Task<CategoryDTO?> Get(int categoryId)
    {
        return await (from category in _context.Categories
                        where category.Id == categoryId
                        select new CategoryDTO(category.Id, category.Name)).FirstOrDefaultAsync();
    }

    public async Task<CategoryDTO> Push(CategoryCreateDTO category)
    {
        var createdCategory = new Category() {Name = category.Name};
        
        await _context.Categories.AddAsync(createdCategory);
        await _context.SaveChangesAsync();
        
        return new CategoryDTO(createdCategory.Id, createdCategory.Name);
    }

    public async Task<Status> Update(CategoryUpdateDTO category)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
       
        if (entity == null)
        {
            return Status.NotFound;
        }

        entity.Name = category.Name;
        
        await _context.SaveChangesAsync();

        return Status.Updated;
    }

}
