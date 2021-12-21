using FluentAssertions.Extensions;
using Server.Model;

namespace VideoOverflow.Server.Tests;


public class ResourcesControllerTests
{
    [Fact]
    public async Task Post_creates_Resource()
    {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var toCreate = new ResourceCreateDTO();
        var repository = new Mock<IResourceRepository>();
        var expected = Created;
        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.Push(toCreate)).ReturnsAsync(expected).Verifiable();
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);
        
        // Act
        var result = await controller.Post(toCreate) as CreatedResult;

        // Assert
        Assert.Equal(201, result?.StatusCode);
        
    }

    [Fact]
    public async Task GetAll_returns_All_Resources_from_repo() {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var expected = Array.Empty<ResourceDTO>();
        var repository = new Mock<IResourceRepository>();
        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.GetAll()).ReturnsAsync(expected);
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);

        // Act
        var actual = await controller.GetAll();

        // Assert
        Assert.Equal(expected, actual);
    }


    [Fact]
    public async Task Get_existing_resource() {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var repository = new Mock<IResourceRepository>();
        var tagRepo = new Mock<ITagRepository>();
        var resource = new ResourceDetailsDTO();
        repository.Setup(m => m.Get(1)).ReturnsAsync(resource);
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);

        // Act
        var response = await controller.Get(1);

        // Assert
        Assert.Equal(resource, response.Value);
    }


    [Fact]
    public async Task Get_given_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var repository = new Mock<IResourceRepository>();
        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.Get(21221121)).ReturnsAsync(default(ResourceDetailsDTO));
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);

        // Act
        var response = await controller.Get(42);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Put_updates_Resource() {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var resource = new ResourceUpdateDTO();
        var repository = new Mock<IResourceRepository>();
        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.Update(resource)).ReturnsAsync(Updated);
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);

        // Act
        var response = await controller.Put(resource);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task Put_given_unknown_resource_returns_NotFound() {

        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var resource = new ResourceUpdateDTO();
        var repository = new Mock<IResourceRepository>();

        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.Update(resource)).ReturnsAsync(NotFound);
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);


        // Act
        var response = await controller.Put(resource);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
}