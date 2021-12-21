using VideoOverflow.Infrastructure.repositories;
using VideoOverflow.Infrastructure.Tests;
using VideoOverflow.Server;

namespace VideoOverflow.Server.Tests;

public class DataFactoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly ResourceRepository _resourceRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly TagRepository _tagRepository;
    private readonly TagSynonymRepository _tagSynonymRepository;
    private readonly UserRepository _userRepository;
    private readonly int _amountOfResources;

    public DataFactoryTests()
    {
        _resourceRepository = new ResourceRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
        _tagRepository = new TagRepository(_context);
        _tagSynonymRepository = new TagSynonymRepository(_context);
        _userRepository = new UserRepository(_context);
        var amount = DataFactory.FillDatabase(_context);
        _amountOfResources = amount.Result;
    }

    [Fact]
    public void ResourcesHaveSpecifiedAmount_GivenFillDatabase_ReturnsTrue()
    {
        var amountOfResources = _resourceRepository.GetAll().Result.Count();

        Assert.Equal(_amountOfResources, amountOfResources);
    }

    [Fact]
    public void ResourcesHaveNoDuplicateCategories_GivenFillDatabase_ReturnsTrue()
    {
        var resources = _resourceRepository.GetAll().Result;
        
        foreach (var resource in resources)
        {
            var categories = new HashSet<string>();
            foreach (var category in resource.Categories)
            {
                categories.Add(category);
            }
            Assert.Equal(categories.Count, resource.Categories.Count);
        }
    }

    [Fact]
    public void ResourcesHaveNoDuplicateTags_GivenFillDatabase_ReturnsTrue()
    {
        var resources = _resourceRepository.GetAll().Result;
        
        foreach (var resource in resources)
        {
            var tags = new HashSet<string>();
            foreach (var category in resource.Tags)
            {
                tags.Add(category);
            }
            Assert.Equal(tags.Count, resource.Tags.Count);
        }
    }

    [Fact]
    public void ResourcesCategoriesAreInCategoriesRelation_GivenFillDatabase_ReturnsTrue()
    {
        var resources = _resourceRepository.GetAll().Result;
        var allCategories = _categoryRepository.GetAll().Result;
        var allCategoryNames = allCategories.Select(x => x.Name).ToList();
        foreach (var resource in resources)
        {
            foreach (var category in resource.Categories)
            {
                Assert.Contains(category, allCategoryNames);
            }
        }
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