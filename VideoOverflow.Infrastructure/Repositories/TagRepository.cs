namespace VideoOverflow.Infrastructure.repositories;

/// <summary>
/// The repository for the user relation
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly IVideoOverflowContext _context;
    
    /// <summary>
    /// Initialize the repository with a given context
    /// </summary>
    /// <param name="context">Context for a DB</param>
    public TagRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all tags in the tags relation in the DB
    /// </summary>
    /// <returns>A collection of all tags in the DB</returns>
    public async Task<IReadOnlyCollection<TagDTO>> GetAll()
    {
        return await _context.Tags.Select(c => new TagDTO(c.Id,
                c.Name,
                c.TagSynonyms.Select(c => c.Name).ToList()))
            .ToListAsync();
    }

    /// <summary>
    /// Gets a tag by it's name variable
    /// </summary>
    /// <param name="tagName">The name of the tag to search for</param>
    /// <returns>The tag with the specified id or null if it doesn't exist</returns>
    public async Task<TagDTO?> GetTagByName(string tagName) {
        return await _context.Tags.Where(t => t.Name.Equals(tagName))
            .Select(c => new TagDTO(c.Id, c.Name, c.TagSynonyms.Select(c => c.Name).ToList())).FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Gets a tag by either it's name or one if it's tagSynonyms if it is similar to the input name
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <returns>A collection of all tags which are similar to the input name</returns>
    public async Task<IReadOnlyCollection<TagDTO>> GetTagByNameAndSynonym(string name) {
        return await _context.Tags.
            Where(t => EF.Functions.TrigramsSimilarity(t.Name, name) > 0.8 || t.TagSynonyms.Any(s => EF.Functions.TrigramsSimilarity(s.Name, name) > 0.8)).
            Select(c => new TagDTO(c.Id, c.Name, c.TagSynonyms.Select(a => a.Name).ToList())).ToListAsync();
    }
    
    /// <summary>
    /// Gets a tag by it's id
    /// </summary>
    /// <param name="tagId">The id of a tag to search for</param>
    /// <returns>A tag with the specified id or null if it doesn't exist</returns>
    public async Task<TagDTO?> Get(int tagId)
    {
        return await _context.Tags.Where(c => c.Id == tagId)
            .Select(s => new TagDTO(s.Id, s.Name, s.TagSynonyms.Select(c => c.Name).ToList()))
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Pushes a tag to the tag relation in the DB
    /// </summary>
    /// <param name="tag">The tag to push to the DB</param>
    /// <returns>The tag that got pushed to the DB</returns>
    public async Task<TagDTO> Push(TagCreateDTO tag)
    {
        var created = new Tag()
        {
            Name = tag.Name,
            TagSynonyms = await GetTagSynonyms(tag.TagSynonyms)
        };

        await _context.Tags.AddAsync(created);
        await _context.SaveChangesAsync();

        return new TagDTO(created.Id, created.Name, created.TagSynonyms.Select(c => c.Name).ToList());
    }

    /// <summary>
    /// Updates a tag in the DB
    /// </summary>
    /// <param name="update">The updated tag</param>
    /// <returns>The status of the update</returns>
    public async Task<Status> Update(TagUpdateDTO update)
    {
        var entity = await (from c in _context.Tags
            where c.Id == update.Id
            select c).FirstOrDefaultAsync();

        if (entity == null)
        {
            return Status.NotFound;
        }
        
        entity.Name = update.Name;
        entity.TagSynonyms = await GetTagSynonyms(update.TagSynonyms);

        await _context.SaveChangesAsync();

        return Status.Updated;
    }

    /// <summary>
    /// Gets all tags by a collections of tagSynonyms' names
    /// </summary>
    /// <param name="tagSynonyms">A collection of the names of tagSynonyms to search for</param>
    /// <returns>A collection of all tagSynonyms found with names in input collection</returns>
    public async Task<ICollection<TagSynonym>> GetTagSynonyms(IEnumerable<string> tagSynonyms)
    {
        var collectionOfTagsynonyms = new Collection<TagSynonym>();

        foreach (var tagSynonym in tagSynonyms)
        {
            var exists = await _context.TagSynonyms.FirstOrDefaultAsync(c => c.Name == tagSynonym);

            if (exists == null)
            {
                exists = new TagSynonym() {Name = tagSynonym, Tags = new Collection<Tag>()};
                await _context.TagSynonyms.AddAsync(exists);
                await _context.SaveChangesAsync();
            }

            collectionOfTagsynonyms.Add(exists);
        }

        return collectionOfTagsynonyms;
    }
}