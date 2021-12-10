using System.Collections.ObjectModel;

namespace VideoOverflow.Infrastructure.Tests;

public class CategoryRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly CategoryRepository _repo;

    public CategoryRepositoryTests()
    {
        _repo = new CategoryRepository(_context);
    }

    [Fact]
    public async Task GetAll_returns_all_categories()
    {
        var programming = new CategoryCreateDTO() {Name = "Programming"};
        var softwareEngineering = new CategoryCreateDTO() {Name = "Software Engineering"};

        await _repo.Push(programming);
        await _repo.Push(softwareEngineering);

        var comments = await _repo.GetAll();

        Assert.Collection(comments, comment => Assert.Equal(new CategoryDTO(1, "Programming"), comment),
            comment => Assert.Equal(new CategoryDTO(2, "Software Engineering"), comment));
    }

    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_categories()
    {
        var actual = await _repo.GetAll();

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task Push_creates_new_category_with_id_and_name()
    {
        var category = new CategoryCreateDTO() {Name = "Software Engineering"};

        var actual = await _repo.Push(category);
        
        Assert.Equal(1, actual.Id);
        Assert.Equal("Software Engineering", actual.Name);
    }

    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        var exists = await _repo.Get(4);

        Assert.Null(exists);
    }

    [Fact]
    public async Task Get_returns_category_for_given_id()
    {
        var category = new CategoryCreateDTO() {Name = "Programming"};

        await _repo.Push(category);

        var expected = new CategoryDTO(1, "Programming");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }


    [Fact]
    public async Task Update_of_existing_category_returns_Updated()
    {
        var category = new CategoryCreateDTO() {Name = "SE"};

        await _repo.Push(category);

        var update = new CategoryUpdateDTO() {Id = 1, Name = "Software Engineering"};

        var actual = await _repo.Update(update);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_category()
    {
        var update = new CategoryUpdateDTO() {Id = 10, Name = "SE"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task Update_changes_name_of_category()
    {
        var category = new CategoryCreateDTO() {Name = "Programming"};

        await _repo.Push(category);

        var update = new CategoryUpdateDTO() {Id = 1, Name = "Database"};

        await _repo.Update(update);

        var expected = new CategoryDTO(1, "Database");

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