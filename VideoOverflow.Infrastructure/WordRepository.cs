namespace VideoOverflow.Infrastructure;

public class WordRepository : IWordRepository
{
    private readonly IVideoOverflowContext _context;

    public WordRepository(IVideoOverflowContext context)
    {
        _context = context;
    }


    public async Task<IReadOnlyCollection<WordDTO>> GetAll()
    {
        return await _context.Words.Select(c => new WordDTO(c.Id,
                c.Hash,
                c.String))
            .ToListAsync();
    }
    public async Task<WordDTO> Push(WordCreateDTO word)
    {
        var created = new Word()
        {
            Hash = word.String.GetHashCode(),
            String = word.String
        };


        await _context.Words.AddAsync(created);
        await _context.SaveChangesAsync();

        return new WordDTO(created.Id, created.Hash, created.String);
    }
}
