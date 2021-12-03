

namespace VideoOverflow.Server.Tests;

public class ResourcesControllerTests
{
    /*
    [Fact]
    public async Task Post_Create_Resource()
    {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var toCreate = new ResourceCreateDTO();
        var created = new ResourceDTO(
            1, 
            ResourceType.VIDEO, 
            "", 
            "", 
            "", 
            "", 
            "", 
            new Collection<string>(),
            new Collection<string>(), 
            new Collection<string>()
            );
        var repository = new Mock<IResourceRepository>();
        repository.Setup(m => m.Push(toCreate)).ReturnsAsync(created);
        var controller = new ResourceController(logger.Object, repository.Object);
        
        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        //Assert.Equal(created, result?.Value);
        //Assert.Equal("Get", result?.ActionName);
        //Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
    }
    */
    
    [Fact]
    public async Task Get_given_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var repository = new Mock<IResourceRepository>();
        repository.Setup(m => m.Get(21221121)).ReturnsAsync(default(ResourceDetailsDTO));
        var controller = new ResourceController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(42);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }
    
    [Fact]
    public async Task Put_given_unknown_resource_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ResourceController>>();
        var resource = new ResourceUpdateDTO();
        var repository = new Mock<IResourceRepository>();
        repository.Setup(m => m.Update(resource)).ReturnsAsync(NotFound);
        var controller = new ResourceController(logger.Object, repository.Object);

        // Act
        var response = await controller.Put(resource);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }


}


/*

public class CharactersControllerTests
{
    [Fact]
    public async Task Create_creates_Character()
    {
        // Arrange
        var logger = new Mock<ILogger<CharactersController>>();
        var toCreate = new CharacterCreateDto();
        var created = new CharacterDetailsDto(1, "Superman", "Clark", "Kent", "Metropolis", Male, 1938, "Reporter", "https://images.com/superman.png", new HashSet<string>());
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
        var controller = new CharactersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        Assert.Equal(created, result?.Value);
        Assert.Equal("Get", result?.ActionName);
        Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
    }

    [Fact]
    public async Task Get_returns_Characters_from_repo()
    {
        // Arrange
        var logger = new Mock<ILogger<CharactersController>>();
        var expected = Array.Empty<CharacterDto>();
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
        var controller = new CharactersController(logger.Object, repository.Object);

        // Act
        var actual = await controller.Get();

        // Assert
        Assert.Equal(expected, actual);
    }



    [Fact]
    public async Task Get_given_existing_returns_character()
    {
        // Arrange
        var logger = new Mock<ILogger<CharactersController>>();
        var repository = new Mock<ICharacterRepository>();
        var character = new CharacterDetailsDto(1, "Superman", "Clark", "Kent", "Metropolis", Male, 1938, "Reporter", "https://images.com/superman.png", new HashSet<string>());
        repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(character);
        var controller = new CharactersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(1);

        // Assert
        Assert.Equal(character, response.Value);
    }

    [Fact]
    public async Task Put_updates_Character()
    {
        // Arrange
        var logger = new Mock<ILogger<CharactersController>>();
        var character = new CharacterUpdateDto();
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(Updated);
        var controller = new CharactersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Put(1, character);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
}

*/