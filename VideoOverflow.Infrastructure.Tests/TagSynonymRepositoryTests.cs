namespace VideoOverflow.Infrastructure.Tests;

public class TagSynonymRepositoryTests
{
    private readonly TagSynonymRepository _repo;
    private readonly VideoOverflowContext _context;


    public TagSynonymRepositoryTests()
    {
        var repo = new RepositoryTestsSetup();
        _context = repo.Context;

        _repo = new TagSynonymRepository(_context);
    }


    [Fact]
    public async Task GetAll_Returns_All_TagSynonyms()
    {
        var tagSynonym1 = new TagSynonymCreateDTO() {Name = "c#"};
        var tagSynonym2 = new TagSynonymCreateDTO() {Name = "C-Sharp"};

        await _repo.Push(tagSynonym1);
        await _repo.Push(tagSynonym2);

        var actual = await _repo.GetAll();
        var expected = new List<TagSynonymDTO>() { new TagSynonymDTO(1, "c#", new List<string>()), new TagSynonymDTO(2, "C-Sharp", new List<string>()) };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_Returns_Empty_List_For_No_existsing_TagSynonyms()
    {
        var actual = await _repo.GetAll();

        var expected = new ReadOnlyCollection<CategoryDTO>(new Collection<CategoryDTO>());

        expected.Should().BeEquivalentTo(actual);
    }


    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        var option = await _repo.Get(4);

        Assert.Null(option);
    }

    [Fact]
    public async Task Get_returns_Category_for_given_id()
    {
        var tagSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(tagSynonym);

        var expected = new TagSynonymDTO(1, "sharp", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }


    [Fact]
    public async Task Update_of_existing_category_returns_StateUpdated()
    {
        var tagSynonym = new TagSynonymCreateDTO() {Name = "sharp"};

        await _repo.Push(tagSynonym);

        var update = new TagSynonymUpdateDTO() {Id = 1, Name = "c-sharp"};

        var actual = await _repo.Update(update);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_category()
    {
        var update = new TagSynonymUpdateDTO() {Id = 10, Name = "c-sharp"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task Update_changes_name_of_category_of_givenID()
    {
        var tagSynonym = new TagSynonymCreateDTO() {Name = "C-sharp"};

        await _repo.Push(tagSynonym);

        var update = new TagSynonymUpdateDTO() {Id = 1, Name = "sharp-C"};

        await _repo.Update(update);

        var expected = new TagSynonymDTO(1, "sharp-C", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }
}