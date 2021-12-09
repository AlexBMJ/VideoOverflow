namespace VideoOverflow.Server.Tests;

public class ResourcesControllerTests
{
    public class CharactersControllerTests
    {
        [Fact]
        public async Task Post_creates_Resource()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var toCreate = new ResourceCreateDTO();
            var created = new ResourceDTO(
                1, ResourceType.VIDEO,
                "https://www.youtube.com",
                "youtube",
                "Youtube",
                "A",
                "english",
                new Collection<string>(),
                new Collection<string>(),
                new Collection<string>()
            );
            var repository = new Mock<IResourceRepository>();
            repository.Setup(m => m.Push(toCreate)).ReturnsAsync(created);
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtActionResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("Get", result?.ActionName);
            Assert.Equal(KeyValuePair.Create("Id", (object?) 1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task GetAll_returns_All_Resources_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var expected = Array.Empty<ResourceDTO>();
            var repository = new Mock<IResourceRepository>();
            repository.Setup(m => m.GetAll()).ReturnsAsync(expected);
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var actual = await controller.GetAll();

            // Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public async Task Get_existing_resource()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var repository = new Mock<IResourceRepository>();
            var resource = new ResourceDetailsDTO();
            repository.Setup(m => m.Get(1)).ReturnsAsync(resource);
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(1);

            // Assert
            Assert.Equal(resource, response.Value);
        }


        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var repository = new Mock<IResourceRepository>();
            repository.Setup(m => m.Get(21221121)).ReturnsAsync(default(ResourceDetailsDTO));
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(42);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }



        
        [Fact]
        public async Task Put_updates_Resource()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var resource = new ResourceUpdateDTO();
            var repository = new Mock<IResourceRepository>();
            repository.Setup(m => m.Update(resource)).ReturnsAsync(Updated);
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(resource);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_resource_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<ResourcesController>>();
            var resource = new ResourceUpdateDTO();
            var repository = new Mock<IResourceRepository>();
            repository.Setup(m => m.Update(resource)).ReturnsAsync(NotFound);
            var controller = new ResourcesController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(resource);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }
    }
}

