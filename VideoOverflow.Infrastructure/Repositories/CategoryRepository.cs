namespace VideoOverflow.Infrastructure.repositories;

/// <summary>
/// The repository for the user relation
/// </summary>
public class CategoryRepository : ICategoryRepository
{

    private readonly IVideoOverflowContext _context;

    /// <summary>
    /// Initialize the repository with a given context
    /// </summary>
    /// <param name="context">Context for a DB</param>
    public CategoryRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all categories from the category relation in the DB
    /// </summary>
    /// <returns>A collection of all categories in the DB</returns>
    public async Task<IReadOnlyCollection<CategoryDTO>> GetAll()
    {
        return (await _context.Categories.Select(category => new CategoryDTO(category.Id, category.Name))
            .ToListAsync()).AsReadOnly();
    }

    /// <summary>
    /// Gets a category based on it's id
    /// </summary>
    /// <param name="categoryId">The id of the category to search for</param>
    /// <returns>The category with the specified id or null if it doesn't exist</returns>
    public async Task<CategoryDTO?> Get(int categoryId)
    {
        return await (from category in _context.Categories
                        where category.Id == categoryId
                        select new CategoryDTO(category.Id, category.Name)).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Pushes a category to the DB
    /// </summary>
    /// <param name="category">The category to push</param>
    /// <returns>The pushed category</returns>
    public async Task<CategoryDTO> Push(CategoryCreateDTO category)
    {
        var createdCategory = new Category() {Name = category.Name};
        
        await _context.Categories.AddAsync(createdCategory);
        await _context.SaveChangesAsync();
        
        return new CategoryDTO(createdCategory.Id, createdCategory.Name);
    }

    /// <summary>
    /// Updates a category in the DB
    /// </summary>
    /// <param name="category">The updated category</param>
    /// <returns>The status of the update</returns>
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
