using System.Collections.ObjectModel;

namespace VideoOverflow.Infrastructure.Tests;

/// <summary>
/// Tests for the categoryRepository
/// </summary>
public class CategoryRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly CategoryRepository _repo;

    /// <summary>
    /// Instantiate each test with a fresh context
    /// </summary>
    public CategoryRepositoryTests()
    {
        _repo = new CategoryRepository(_context);
    }

    /// <summary>
    /// Test the categoryRepository's getAll method for a non empty DB
    /// to ensure all categories are returned
    /// </summary>
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

    /// <summary>
    /// Test the categoryRepository's getAll method for an empty DB
    /// to ensure an empty collection is returned
    /// </summary>
    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_categories()
    {
        var actual = await _repo.GetAll();

        actual.Should().BeEmpty();
    }

    /// <summary>
    /// Test the categoryRepository's push method
    /// to ensure the category is created correctly in the DB
    /// </summary>
    [Fact]
    public async Task Push_creates_new_category_with_id_and_name()
    {
        var category = new CategoryCreateDTO() {Name = "Software Engineering"};

        var actual = await _repo.Push(category);
        
        Assert.Equal(1, actual.Id);
        Assert.Equal("Software Engineering", actual.Name);
    }

    /// <summary>
    /// Test the categoryRepository's get method for a non existing category
    /// to ensure it returns null
    /// </summary>
    [Fact]
    public async void Get_returns_null_for_non_existing_id()
    {
        var exists = await _repo.Get(4);

        Assert.Null(exists);
    }

    /// <summary>
    /// Test the categoryRepository's get method for an existing category
    /// to ensure the correct category is returned from the DB
    /// </summary>
    [Fact]
    public async Task Get_returns_category_for_given_id()
    {
        var category = new CategoryCreateDTO() {Name = "Programming"};

        await _repo.Push(category);

        var expected = new CategoryDTO(1, "Programming");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Test the categoryRepository's update method for an existing category
    /// to ensure the action result updated is returned
    /// </summary>
    [Fact]
    public async Task Update_of_existing_category_returns_Updated()
    {
        var category = new CategoryCreateDTO() {Name = "SE"};

        await _repo.Push(category);

        var update = new CategoryUpdateDTO() {Id = 1, Name = "Software Engineering"};

        var actual = await _repo.Update(update);

        Assert.Equal(Status.Updated, actual);
    }

    /// <summary>
    /// Test the categoryRepository's update method for a non existing category
    /// to ensure the notfound action result is returned
    /// </summary>
    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_category()
    {
        var update = new CategoryUpdateDTO() {Id = 10, Name = "SE"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    /// <summary>
    /// Test the categoryRepository's update method for an existing category
    /// to ensure the category is updated accordingly in the DB
    /// </summary>
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