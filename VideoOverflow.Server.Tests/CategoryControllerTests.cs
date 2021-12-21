namespace VideoOverflow.Server.Tests;

/// <summary>
/// Tests for our categoryController
/// </summary>
public class CategoryControllerTests
{
    /// <summary>
    /// Test our categoryController's get all methods
    /// to ensure it returns all entries in the category relation in the DB
    /// </summary>
    [Fact]
    public async Task GetAll_Returns_AllCategories()
    {
        var logger = new Mock<ILogger<CategoryController>>();
        var repository = new Mock<ICategoryRepository>();
        var expected = Array.Empty<CategoryDTO>();
        repository.Setup(m => m.GetAll()).ReturnsAsync(expected);
        var controller = new CategoryController(logger.Object, repository.Object);
        
        var actual = await controller.GetAll();
        
        Assert.Equal(expected, actual);
    }
}