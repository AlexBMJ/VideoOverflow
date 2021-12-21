namespace VideoOverflow.Infrastructure.Tests;

/// <summary>
/// Tests for our tagSynonymRepository
/// </summary>
public class TagSynonymRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly TagSynonymRepository _repo;
    
    /// <summary>
    /// Instantiate each test with a fresh repository
    /// </summary>
    public TagSynonymRepositoryTests()
    {
        _repo = new TagSynonymRepository(_context);
    }

    [Fact]
    public async Task Push_returns_pushed_tag()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "c#"};

        var tagSyn = await _repo.Push(cSharpSynonym);
        await _repo.Push(new TagSynonymCreateDTO() {Name = "123"});
        await _repo.Push(new TagSynonymCreateDTO() {Name = "abc"});
        var expected = new TagSynonymDTO(1, "c#", new List<string>());
        expected.Should().BeEquivalentTo(tagSyn);
    }
    [Fact]
    public async Task GetTagBySynName_returns_correct_tag_out_of_many()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "c#"};
        var cDashSharpSynonym = new TagSynonymCreateDTO() {Name = "C-Sharp"};
        var cSharpLiteralSynonym = new TagSynonymCreateDTO() {Name = "cSharp"};
        var cDashBluntSynonym = new TagSynonymCreateDTO() {Name = "C-Blunt"};

        await _repo.Push(cSharpLiteralSynonym);
        await _repo.Push(cDashBluntSynonym);
        await _repo.Push(cSharpSynonym);
        await _repo.Push(cDashSharpSynonym);

        var actual = await _repo.GetTagSynByName("c#");

        var expected = new TagSynonymDTO(3, "c#", new List<string>());

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the tagSynonymRepository's getAll method
    /// to ensure it returns all tagSynonyms in the DB
    /// </summary>
    [Fact]
    public async Task GetAll_returns_all_TagSynonyms()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "c#"};
        var cDashSharpSynonym = new TagSynonymCreateDTO() {Name = "C-Sharp"};

        await _repo.Push(cSharpSynonym);
        await _repo.Push(cDashSharpSynonym);

        var actual = await _repo.GetAll();

        var expected = new Collection<TagSynonymDTO>() {
            new TagSynonymDTO(1, "c#", new List<string>()),
            new TagSynonymDTO(2, "C-Sharp", new List<string>())
        };

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the tagSynonymRepository's getAll method for an empty DB
    /// to ensure it returns an empty collection
    /// </summary>
    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_TagSynonyms()
    {
        var actual = await _repo.GetAll();
        
        actual.Should().BeEmpty();
    }

    /// <summary>
    /// Test the tagSynonymRepository's get method for a non existing tagSynonym
    /// to ensure it returns null
    /// </summary>
    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        Assert.Null(await _repo.Get(4));
    }
    
    /// <summary>
    /// Test the tagSynonymRepository's get method for an existing tagSynonym
    /// to ensure it returns the correct tagSynonym
    /// </summary>
    [Fact]
    public async Task Get_returns_tagSynonym()
    {
        var sharpSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(sharpSynonym);

        var expected = new TagSynonymDTO(1, "sharp", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the tagSynonymRepository's update method for an existing tagSynonym
    /// to ensure it returns the updated action result
    /// </summary>
    [Fact]
    public async Task Update_of_existing_tagSynonym_returns_Updated()
    {
        var sharpSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(sharpSynonym);

        var synonymUpdate = new TagSynonymUpdateDTO() {Id = 1, Name = "c-sharp"};

        var actual = await _repo.Update(synonymUpdate);

        Assert.Equal(Status.Updated, actual);
    }

    /// <summary>
    /// Test the tagSynonymRepository's updated method for a non existing tagsynonym
    /// to ensure it returns the action result notfound
    /// </summary>
    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_tagSynonym()
    {
        var update = new TagSynonymUpdateDTO() {Id = 10, Name = "c-sharp"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    /// <summary>
    /// Test the tagSynonymRepository's update method for an existing tagSynonym
    /// to ensure the correct tagSynonym is updated accordingly
    /// </summary>
    [Fact]
    public async Task Update_changes_name_of_tagSynonym()
    {
        var cSharpSynonym = new TagSynonymCreateDTO() {Name = "C-sharp"};

        await _repo.Push(cSharpSynonym);

        var update = new TagSynonymUpdateDTO() {Id = 1, Name = "sharp-C"};

        await _repo.Update(update);

        var expected = new TagSynonymDTO(1, "sharp-C", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }
    
    /* Dispose code has been taken from  https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CityRepositoryTests.cs*/
    private bool _disposed;
    /// <summary>
    /// If we haven't disposed yet, we start disposing our context
    /// </summary>
    /// <param name="disposing"> Boolean for if we have disposed yet</param>
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