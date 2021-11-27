namespace VideoOverflow.Infrastructure.Tests;

public class CommentRepositoryTests
{

    private readonly VideoOverflowContext _context;
    private readonly CommentRepository _repo;
    public CommentRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();

        context.SaveChanges();
        
        _context = context;
        _repo = new CommentRepository(_context);
    }
    
    [Fact]
    public async Task GetAll_Returns_All_Categories()
    {
        var comment1 = new CommentCreateDTO() {Content = "This docker tutorial is smooth"};
        var comment2 = new CommentCreateDTO() {Content = "Very helpful guide for beginners!"};

        await _repo.Push(comment1);
        await _repo.Push(comment2);

        var comments = await _repo.GetAll();

        Assert.Collection(comments, comment => Assert.Equal(new CommentDTO(1, "This docker tutorial is smooth"), comment),
            comment => Assert.Equal(new CommentDTO(2, "Very helpful guide for beginners!"), comment));
    }

    [Fact]
    public async Task GetAll_Returns_Empty_List_For_No_existsing_Comments()
    {
        var actual = await _repo.GetAll();

        var expected = new ReadOnlyCollection<CommentDTO>(new Collection<CommentDTO>());

        expected.Should().BeEquivalentTo(actual);
    }
    
    [Fact]
    public async Task Push_creates_new_comment_with_autogenerated_id_and_written_content()
    {
        var comment = new CommentCreateDTO() {Content = "Nice Video!"};

        var actual = await _repo.Push(comment);
        
        Assert.Equal(1, actual.Id);
        Assert.Equal("Nice Video!", actual.Content);
    }

    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        var comment = await _repo.Get(4);
        
        Assert.Null(comment);
    }


    [Fact]
    public async Task Update_of_existing_comment_returns_StateUpdated()
    {
        var comment = new CommentCreateDTO() {Content = "This is an awesome project!"};

        await _repo.Push(comment);

        var update = new CommentUpdateDTO() {Id = 1, Content = "Yes, i mean it's crazy!"};

        var actual = await _repo.Update(update);
        
        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_category()
    {
        var update = new CommentUpdateDTO() {Id = 10, Content = "Am i trying to change an non-existing comment?"};

        var response = await _repo.Update(update);
        
        Assert.Equal(Status.NotFound, response);
    }
    
    
}