namespace VideoOverflow.Infrastructure;

public class TagRepository : ITagRepository
{
    private readonly IVideoOverflowContext _context;

    public TagRepository(IVideoOverflowContext context)
    {
        _context = context;
    }


    public async Task<IReadOnlyCollection<TagDTO>> GetAll()
    {
        return await _context.Tags.Select(c => new TagDTO(c.Id,
                c.Name,
                c.TagSynonyms.Select(c => c.Name).ToList()))
            .ToListAsync();
    }

    public async Task<TagDTO?> Get(int tagId)
    {
        return await _context.Tags.Where(c => c.Id == tagId)
            .Select(s => new TagDTO(s.Id, s.Name, s.TagSynonyms.Select(c => c.Name).ToList()))
            .FirstOrDefaultAsync();
    }

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