namespace VideoOverflow.Infrastructure.Tests;

public class TagRepositoryTests
{
    private readonly VideoOverflowContext _context;
    private readonly TagRepository _repo;

    public TagRepositoryTests()
    {
        var repo = new RepositoryTestsSetup();
        _context = repo.Context;

        _repo = new TagRepository(_context);
    }
}