namespace VideoOverflow.Infrastructure;

public class TagSynonymRepository : ITagSynonymRepository
{
    private readonly IVideoOverflowContext _context;


    public TagSynonymRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<TagSynonymDTO>> GetAll()
    {
        return await _context.TagSynonyms.Select(c => new TagSynonymDTO(c.Id, c.Name)).ToListAsync();
    }

    public async Task<TagSynonymDTO?> Get(int id)
    {
        return await _context.TagSynonyms.Where(ts => ts.Id == id).Select(ts => new TagSynonymDTO(ts.Id, ts.Name)).FirstOrDefaultAsync();
    }

    public async Task<TagSynonymDTO> Push(TagSynonymCreateDTO create)
    {
        var created = new TagSynonym()
        {
            Name = create.Name,
            Tags = new List<Tag>()
        };

        await _context.TagSynonyms.AddAsync(created);
        await _context.SaveChangesAsync();
   

        return new TagSynonymDTO(created.Id, created.Name);
    }

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