using FluentAssertions.Extensions;
using VideoOverflow.Server.Model;

namespace VideoOverflow.Server.Tests;

/// <summary>
/// Tests for our resourceController
/// </summary>
public class ResourcesControllerTests
{
    /// <summary>
    /// Test the resourceController's post method
    /// to ensure it creates a post in the DB
    /// </summary>
    [Fact]
    public async Task Post_creates_Resource()
    {

        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var toCreate = new ResourceCreateDTO();
        var created = new ResourceDTO(
            1, ResourceType.Video,
            "https://www.youtube.com",
            "youtube",
            "Youtube",
            DateTime.Parse("08-08-2012").AsUtc(),
            "A",
            "english",
            new Collection<string>(),
            new Collection<string>(),
            new Collection<string>()
        );
        var repository = new Mock<IResourceRepository>();
      
        var tagRepo = new Mock<ITagRepository>();
        repository.Setup(m => m.Push(toCreate)).ReturnsAsync(created);
        var controller = new ResourceController(logger.Object, repository.Object, tagRepo.Object);


        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        Assert.Equal(created, result?.Value);
        Assert.Equal("Get", result?.ActionName);
        Assert.Equal(KeyValuePair.Create("Id", (object?) 1), result?.RouteValues?.Single());


    }

    /// <summary>
    /// Test the resourceController's getAll method
    /// to ensure it returns all entries in the DB
    /// </summary>
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

    /// <summary>
    /// Test the resourceController's get method on an existing resource
    /// to ensure the method returns the correct resource
    /// </summary>
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

    /// <summary>
    /// Test the resourceController's get method on a non existing resource
    /// to ensure it returns a notfound actionresult
    /// </summary>
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

    /// <summary>
    /// Test the resourceController's put method on an existing resource
    /// to ensure that the resource is updated when finished
    /// </summary>
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

    /// <summary>
    /// Test the resourceController's put method on a non existing resource
    /// to ensure it returns a notfound actionresult.
    /// </summary>
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