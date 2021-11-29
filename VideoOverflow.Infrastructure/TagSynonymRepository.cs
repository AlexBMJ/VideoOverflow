namespace VideoOverflow.Infrastructure;

public class TagSynonymRepository : ITagSynonymRepository
{
    private readonly IVideoOverflowContext _context;


    public TagSynonymRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagSynonymDTO>> GetAll()
    {
        return await _context.TagSynonyms.Select(c => new TagSynonymDTO(c.Id, c.Name)).ToListAsync();
    }

    public async Task<TagSynonymDTO?> Get(int id)
    {
        var entity = await _context.TagSynonyms.Where(c => c.Id == id).Select(c => c).FirstOrDefaultAsync();
        // add code 

        return entity == null ? null : new TagSynonymDTO(entity.Id, entity.Name);
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
        // add code

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