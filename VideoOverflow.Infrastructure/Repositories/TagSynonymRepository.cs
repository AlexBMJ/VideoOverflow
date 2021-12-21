namespace VideoOverflow.Infrastructure.repositories;

/// <summary>
/// The repository for the user relation
/// </summary>
public class TagSynonymRepository : ITagSynonymRepository
{
    private readonly IVideoOverflowContext _context;

    /// <summary>
    /// Initialize the repository with a given context
    /// </summary>
    /// <param name="context">Context for a DB</param>
    public TagSynonymRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all the tagSynonyms in the relation in the DB
    /// </summary>
    /// <returns>A collection of all tagSynonyms in the DB</returns>
    public async Task<IReadOnlyCollection<TagSynonymDTO>> GetAll()
    {
        return await _context.TagSynonyms.Select(c => new TagSynonymDTO(c.Id, c.Name, c.Tags.Select(t=>t.Name).ToList())).ToListAsync();
    }

    /// <summary>
    /// Gets a tagSynonym by it's name variable
    /// </summary>
    /// <param name="tagSyn">The name of the tagSynonym to search for</param>
    /// <returns>The tagSynonym with the specified name or null if it doesn't exist</returns>
    public async Task<TagSynonymDTO?> GetTagSynByName(string tagSyn) {
        return await _context.TagSynonyms.Where(t => t.Name.Equals(tagSyn))
            .Select(c => new TagSynonymDTO(c.Id, c.Name, c.Tags.Select(t=>t.Name).ToList())).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a tagSynonym by it's id
    /// </summary>
    /// <param name="id">The id of the tagSynonym to search for</param>
    /// <returns>The tagSynonym with the specified id or null if it doesn't exist</returns>
    public async Task<TagSynonymDTO?> Get(int id)
    {
        return await _context.TagSynonyms.Where(ts => ts.Id == id).Select(ts => new TagSynonymDTO(ts.Id, ts.Name, ts.Tags.Select(t=>t.Name).ToList())).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Pushes a tagSynonym to the relation in the DB
    /// </summary>
    /// <param name="create">The tagSynonym to push to the DB</param>
    /// <returns>The tagSynonym that got pushed</returns>
    public async Task<TagSynonymDTO> Push(TagSynonymCreateDTO create)
    {
        var created = new TagSynonym()
        {
            Name = create.Name,
            Tags = new List<Tag>()
        };

        await _context.TagSynonyms.AddAsync(created);
        await _context.SaveChangesAsync();
   

        return new TagSynonymDTO(created.Id, created.Name, created.Tags.Select(t=>t.Name).ToList());
    }

    /// <summary>
    /// Updates a tagSynonym to the relation in the DB
    /// </summary>
    /// <param name="update">The updated tagSynonym</param>
    /// <returns>The status of the update</returns>
    public async Task<Status> Update(TagSynonymUpdateDTO update)
    {
        var entity = await _context.TagSynonyms.Where(c => c.Id == update.Id)
            .Select(c => c).FirstOrDefaultAsync();
        // add code
        if (entity == null)
        {
            return Status.NotFound;
        }

        if (update.Name != entity.Name)
        {
            entity.Name = update.Name;
        }

        await _context.SaveChangesAsync();

        return Status.Updated;
    }
}