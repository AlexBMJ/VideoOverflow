namespace VideoOverflow.Server.Tests;

public class CategoryControllerTests
{
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