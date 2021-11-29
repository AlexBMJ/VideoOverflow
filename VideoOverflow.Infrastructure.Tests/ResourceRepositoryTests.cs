namespace VideoOverflow.Infrastructure.Tests;

public class ResourceRepositoryTests : IDisposable
{
    private readonly VideoOverflowContext _context;
    private readonly ResourceRepository _repo;
    private ResourceCreateDTO _resource;
    private DateTime Created = DateTime.Now;

    public ResourceRepositoryTests()
    {
        var setup = new RepositoryTestsSetup();
        _context = setup.Context;
        _repo = new ResourceRepository(_context);

        _resource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "Deniz",
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            MaterialType = ResourceType.VIDEO,
            Categories = new Collection<string>() {"Programming"},
            Tags = new Collection<string>() {"C#"},
        };

        
    }

    [Fact]
    public async Task Push_Creates_New_Resource_And_Returns_ResourceDTO_With_Id()
    {
        // _resource refers to the ResourceCreateDTO in the constructor to avoid duplicate

        var actual = await _repo.Push(_resource);

        var expected = new ResourceDTO(
            1,
            ResourceType.VIDEO,
            "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            "learnit.itu.dk",
            "My first Page",
            "Deniz",
            "Danish",
            new Collection<string>() {"C#"},
            new Collection<string>() {"Programming"},
            new Collection<string>());

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_Returns_null_For_Non_Existing_Resource()
    {
        var exists = await _repo.Get(1000);

        Assert.Null(exists);
    }

    [Fact]
    public async Task Get_Returns_ResourceDTO_For_GivenId()
    {
        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            SkillLevel = 1,
            MaterialType = ResourceType.VIDEO,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            SiteTitle = "My first Page",
            ContentSource = "learnit.itu.dk",
            Author = "Deniz",
            Language = "Danish",
            Tags = new Collection<string>() {"C#"},
            Categories = new Collection<string>() {"Programming"},
            Comments = new Collection<string>()
        };

        await _repo.Push(_resource);

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_Returns_All_Resources()
    {
        await _repo.Push(_resource);

        var resource2 = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "OndFisk",
            SiteTitle = "Opret et Microsoft Teams webinar",
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            Language = "Danish",
            MaterialType = ResourceType.TEXTUAL_GUIDE,
            Categories = new Collection<string>() { },
            Tags = new Collection<string>() { },
        };

        await _repo.Push(resource2);

        var actual = await _repo.GetAll();


        var firstResourceDto = new ResourceDTO(
            1,
            ResourceType.VIDEO,
            "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            "learnit.itu.dk",
            "My first Page",
            "Deniz",
            "Danish",
            new Collection<string>() {"C#"},
            new Collection<string>() {"Programming"},
            new Collection<string>());

        var secondResourceDto = new ResourceDTO(
            2,
            ResourceType.TEXTUAL_GUIDE,
            "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            "docs.microsoft.com",
            "Opret et Microsoft Teams webinar",
            "OndFisk",
            "Danish",
            new Collection<string>() { },
            new Collection<string>() { },
            new Collection<string>()
        );

        var expected = new Collection<ResourceDTO>() {firstResourceDto, secondResourceDto};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_Returns_null_for_Empty_List()
    {
        var actual = await _repo.GetAll();

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task Push_resource_With_new_tag_creates_new_tags_to_DB()
    {
        var resource = new ResourceCreateDTO()
        {
            Created = Created,
            MaterialType = ResourceType.TEXTUAL_GUIDE,
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            Categories = new Collection<string>(),
            Tags = new Collection<string>() {"C#", "Java"}
        };
        await _repo.Push(resource);

        var actual = await (from tag in _context.Tags select tag.Name).ToListAsync();

        var expected = new List<string>() {"C#", "Java"};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Update_returns_StatusNotFound_for_No_Existing_resource()
    {
        await _repo.Push(_resource);

        var update = new ResourceUpdateDTO()
        {
            Id = 100
        };

        var actual = await _repo.Update(update);

        Assert.Equal(Status.NotFound, actual);
    }

    [Fact]
    public async Task Update_returns_Status_Updated_For_Updated_Resource()
    {
        await _repo.Push(_resource);

        var update = new ResourceUpdateDTO()
        {
            Id = 1,
            Created = Created,
            LixNumber = 45,
            Author = "OndFisk",
            SiteTitle = "Changed from my first page",
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            Language = "English",
            Tags = new List<string>() {"C#"},
            Categories = new List<string>() {"Programming"},
            MaterialType = ResourceType.TEXTUAL_GUIDE
        };

        var response = await _repo.Update(update);

        Assert.Equal(Status.Updated, response);

        var actual = await _repo.Get(1);

        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            LixNumber = 45,
            SkillLevel = 4,
            MaterialType = ResourceType.TEXTUAL_GUIDE,
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            ContentSource = "docs.microsoft.com",
            SiteTitle = "Changed from my first page",
            Author = "OndFisk",
            Language = "English",
            Tags = new List<string>() {"C#"},
            Categories = new List<string>() {"Programming"},
            Comments = new List<string>() 
        };

        expected.Should().BeEquivalentTo(actual);
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
