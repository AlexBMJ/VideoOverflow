
namespace VideoOverflow.Infrastructure.Tests;

public class ResourceRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly ResourceRepository _repo;
    private readonly ResourceCreateDTO _resource;

    public ResourceRepositoryTests()
    {
        _repo = new ResourceRepository(_context);

        _resource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "Deniz",
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            Categories = new Collection<string>() {"Programming"},
            Tags = new Collection<string>() {"C#"}
        };
    }

    [Fact]
    public async Task Push_returns_resourceDTO_with_Id()
    {
        // _resource refers to the ResourceCreateDTO in the constructor to avoid duplicate

        var actual = await _repo.Push(_resource);

        var expected = new ResourceDTO(
            1,
            ResourceType.Video,
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
    public async Task Get_returns_null_for_non_existing_resource()
    {
        var exists = await _repo.Get(1000);

        Assert.True(exists.IsNone);
    }

    [Fact]
    public async Task Get_returns_resourceDTO_for_givenId()
    {
        await _repo.Push(_resource);

        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            SkillLevel = 1,
            MaterialType = ResourceType.Video,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            SiteTitle = "My first Page",
            ContentSource = "learnit.itu.dk",
            Author = "Deniz",
            Language = "Danish",
            Tags = new Collection<string>() {"C#"},
            Categories = new Collection<string>() {"Programming"},
            Comments = new Collection<string>()
        };

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual.Value);
    }

    [Fact]
    public async Task Get_returns_added_comments_to_resource_after_initialization()
    {
        var comment = new Comment()
        {
            Content = "I just added this comment",
            CreatedBy = 0,
            AttachedToResource = 1
        };

        await _repo.Push(_resource);

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var resource = await _context.Resources.FindAsync(1);
        resource.Comments = GetComments(1);

        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            SkillLevel = 1,
            MaterialType = ResourceType.Video,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            SiteTitle = "My first Page",
            ContentSource = "learnit.itu.dk",
            Author = "Deniz",
            Language = "Danish",
            Tags = new Collection<string>() {"C#"},
            Categories = new Collection<string>() {"Programming"},
            Comments = new Collection<string>() {"I just added this comment"}
        };
        
        var actual = await _repo.Get(1);
        
        expected.Should().BeEquivalentTo(actual.Value);
    }

    [Fact]
    public async Task GetAll_returns_all_resources()
    {
        await _repo.Push(_resource);

        var microsoftResource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "OndFisk",
            SiteTitle = "Opret et Microsoft Teams webinar",
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            Language = "Danish",
            MaterialType = ResourceType.Article,
            Categories = new Collection<string>() { },
            Tags = new Collection<string>() { },
        };

        await _repo.Push(microsoftResource);

        var actual = await _repo.GetAll();

        var learnItResourceDTO = new ResourceDTO(
            1,
            ResourceType.Video,
            "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            "learnit.itu.dk",
            "My first Page",
            "Deniz",
            "Danish",
            new Collection<string>() {"C#"},
            new Collection<string>() {"Programming"},
            new Collection<string>());

        var microsoftResourceDTO = new ResourceDTO(
            2,
            ResourceType.Article,
            "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            "docs.microsoft.com",
            "Opret et Microsoft Teams webinar",
            "OndFisk",
            "Danish",
            new Collection<string>() { },
            new Collection<string>() { },
            new Collection<string>()
        );

        var expected = new Collection<ResourceDTO>() {learnItResourceDTO, microsoftResourceDTO};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_returns_empty_for_no_resources()
    {
        var actual = await _repo.GetAll();

        actual.Should().BeEmpty();
    }

    [Theory]
    [InlineData(55, 5)]
    [InlineData(45, 4)]
    [InlineData(35, 3)]
    [InlineData(25, 2)]
    public async Task Resource_given_LixLevel_returns_skillLevel(int lixLevel, int expectedSkillLevel)
    {
        await _repo.Push(_resource);

        var entity = await _context.Resources.FindAsync(1);

        entity.LixNumber = lixLevel;

        var expected =  _repo.Get(1).Result.Value.SkillLevel;
        var actual = expectedSkillLevel;

        Assert.Equal(expected, actual);
        
    }

    [Fact]
    public async Task Push_resource_with_new_tags_creates_new_tags_in_DB()
    {
        await _repo.Push(_resource);

        var actual = await _context.Tags.Select(c => c.Name).ToListAsync();

        var expected = new Collection<string>() {"C#"};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_sets_author_to_unknown_if_author_is_null()
    {
        var resource = new ResourceCreateDTO()
        {
            Created = Created,
            MaterialType = ResourceType.Article,
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            Categories = new Collection<string>(),
            Tags = new Collection<string>() {"C#", "Java"},
            Author = null
        };

        await _repo.Push(resource);

        var actual = await _repo.Get(1);
        
        "Unknown".Should().BeEquivalentTo(actual.Value.Author);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_no_existing_resource()
    {
        var actual = await _repo.Update(new ResourceUpdateDTO() {Id = 100});

        Assert.Equal(Status.NotFound, actual);
    }

    [Fact]
    public async Task Update_returns_Updated_for_updated_resource()
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
            MaterialType = ResourceType.Article
        };

        var response = await _repo.Update(update);

        Assert.Equal(Status.Updated, response);
    }

    [Fact]
    public async Task Update_updates_all_attributes()
    {
        await _repo.Push(_resource);

        var updated = new ResourceUpdateDTO()
        {
            Id = 1,
            Author = null,
            LixNumber = 45,
            Language = "English",
            MaterialType = ResourceType.Article,
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            Categories = new Collection<string>(),
            Tags = new Collection<string>(),
            SiteTitle = "Changed to this topic",
            Created = Created
        };

        await _repo.Update(updated);

        var actual = await _repo.Get(1);

        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            LixNumber = 45,
            SkillLevel = 4,
            MaterialType = ResourceType.Article,
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            ContentSource = "docs.microsoft.com",
            SiteTitle = "Changed to this topic",
            Author = "Unknown",
            Language = "English",
            Tags = new Collection<string>() { },
            Categories = new Collection<string>() { },
            Comments = new Collection<string>()
        };

        expected.Should().BeEquivalentTo(actual.Value);
    }
    
    [Fact]
    public async Task Push_given_negative_lixNumber_sets_lixNumber_to_0_and_returns_skillLevel_equals_1()
    {

        var resource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "Deniz",
            SiteTitle = "My first Page",
            LixNumber = -121212,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            Categories = new Collection<string>() {"Programming"},
            Tags = new Collection<string>() {"C#"}
        };

        await _repo.Push(resource);

        var expectedObj = await _repo.Get(1);
        
        Assert.Equal(expectedObj.Value.LixNumber, 0);
        Assert.Equal(expectedObj.Value.SkillLevel, 1);
    }
    
    [Fact]
    public async Task Push_given_invalid_url_throws_exception()
    {

        var resource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "Deniz",
            SiteTitle = "My first Page",
            LixNumber = -121212,
            SiteUrl = "ThisIsAnInvalidURL.com",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            Categories = new Collection<string>() {"Programming"},
            Tags = new Collection<string>() {"C#"}
        };
        
        await Assert.ThrowsAsync<Exception>(() => _repo.Push(resource));
    }

    private ICollection<Comment> GetComments(int resourceId)
    {
        var collection = new Collection<Comment>();

        foreach (var comment in _context.Comments)
        {
            if (comment.AttachedToResource == resourceId)
            {
                collection.Add(
                    new Comment() {Content = comment.Content, CreatedBy = 0, AttachedToResource = resourceId});
            }
        }
        return collection;
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
       
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}