using System.Collections.ObjectModel;
using System.Linq;
using Moq;

namespace VideoOverflow.Infrastructure;

public class TagRepository
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
            Name = tag.Name, TagSynonyms = tag.TagSynonyms
                .Select(c => new TagSynonym() {Name = c}).ToList()
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

        var tagSynonyms = new Collection<TagSynonym>();
        
        foreach (var synonym in update.TagSynonyms)
        {
            tagSynonyms.Add(new TagSynonym(){Name = synonym});
        }
        
        entity.Name = update.Name;
        entity.TagSynonyms = tagSynonyms;

        await _context.SaveChangesAsync();

        return Status.Updated;
    }
}