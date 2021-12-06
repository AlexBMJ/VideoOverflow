namespace VideoOverflow.Infrastructure.Tests;

public class WordRepositoryTests
{
    private readonly VideoOverflowContext _context;
    private readonly WordRepository _repo;

    public WordRepositoryTests()
    {
        var repo = new RepositoryTestsSetup();
        _context = repo.Context;

        _repo = new WordRepository(_context);
    }

    [Fact]
    public async Task GetAll_Returns_All_Words()
    {
        var word1 = new WordCreateDTO() {String = "Hi"};

        var word2 = new WordCreateDTO() {String = "John"};;

        var word3 = new WordCreateDTO() {String = "Johnny"};;

        await _repo.Push(word1);
        await _repo.Push(word2);
        await _repo.Push(word3);

        var wordDTO1 = new WordDTO(1, "Hi".GetHashCode(),
            "Hi");

        var wordDTO2 = new WordDTO(2, "John".GetHashCode(),
            "John");

        var wordDTO3 = new WordDTO(3, "Johnny".GetHashCode(),
            "Johnny");

        var actual = await _repo.GetAll();

        var expected = new Collection<WordDTO>() {wordDTO1, wordDTO2, wordDTO3};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_Empty_Table()
    {
        var actual = await _repo.GetAll();

        var expected = new Collection<WordDTO>();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_Existing_Word_Returns_WordDTO_for_given_id()
    {
        var word1 = new WordCreateDTO() {String = "Hi"};

        var word2 = new WordCreateDTO() {String = "Hi"};;

        var firstWordPushed = await _repo.Push(word1);
        var actual = await _repo.Push(word2);


        var expected = new WordDTO(2, "Hi".GetHashCode(),
            "Hi");

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_Creates_Id_In_DB_and_returns_WordDTO_for_created_entity()
    {
        var word1 = new WordCreateDTO() {String = "Hi"};

        var word2 = new WordCreateDTO() {String = "John"};;

        var actual1 = await _repo.Push(word1);
        var actual2 = await _repo.Push(word2);

        var expected1 = new WordDTO(1, "Hi".GetHashCode(),
            "Hi");
        var expected2 = new WordDTO(2, "John".GetHashCode(),
            "John");

        expected1.Should().BeEquivalentTo(actual1);
        expected2.Should().BeEquivalentTo(actual2);
    }
}