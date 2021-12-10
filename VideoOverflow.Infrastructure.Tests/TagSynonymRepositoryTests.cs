namespace VideoOverflow.Infrastructure.Tests;

public class TagSynonymRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly TagSynonymRepository _repo;
    
    public TagSynonymRepositoryTests()
    {
        _repo = new TagSynonymRepository(_context);
    }


    [Fact]
    public async Task GetAll_returns_all_TagSynonyms()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "c#"};
        var cDashSharpSynonym = new TagSynonymCreateDTO() {Name = "C-Sharp"};

        await _repo.Push(cSharpSynonym);
        await _repo.Push(cDashSharpSynonym);

        var actual = await _repo.GetAll();

        var expected = new Collection<TagSynonymDTO>() {
            new TagSynonymDTO(1, "c#"),
            new TagSynonymDTO(2, "C-Sharp")
        };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_TagSynonyms()
    {
        var actual = await _repo.GetAll();
        
        actual.Should().BeEmpty();
    }


    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        Assert.Null(await _repo.Get(4));
    }

    [Fact]
    public async Task Get_returns_tagSynonym()
    {
        var sharpSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(sharpSynonym);

        var expected = new TagSynonymDTO(1, "sharp");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }


    [Fact]
    public async Task Update_of_existing_tagSynonym_returns_Updated()
    {
        var sharpSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(sharpSynonym);

        var synonymUpdate = new TagSynonymUpdateDTO() {Id = 1, Name = "c-sharp"};

        var actual = await _repo.Update(synonymUpdate);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_tagSynonym()
    {
        var update = new TagSynonymUpdateDTO() {Id = 10, Name = "c-sharp"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task Update_changes_name_of_tagSynonym()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "C-sharp"};

        await _repo.Push(cSharpSynonym);

        var update = new TagSynonymUpdateDTO() {Id = 1, Name = "sharp-C"};

        await _repo.Update(update);

        var expected = new TagSynonymDTO(1, "sharp-C");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }
    
    /* Dispose code has been taken from  https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CityRepositoryTests.cs*/
    private bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}