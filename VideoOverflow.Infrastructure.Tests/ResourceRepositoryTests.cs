using Npgsql.Replication.PgOutput;

namespace VideoOverflow.Infrastructure.Tests;

public class ResourceRepositoryTests : RepositoryTestsSetup
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
            MaterialType = ResourceType.VIDEO,
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
            Comments = new Collection<string>() {"I just added this comment"}
        };

        resource.Comments = GetComments(expected.Id);

        var actual = await _repo.Get(1);


        expected.Should().BeEquivalentTo(actual.Value);
    }

    [Fact]
    public async Task GetAll_returns_all_resources()
    {
        await _repo.Push(_resource);

        var mircrosoftResource = new ResourceCreateDTO()
        {
            Created = Created,
            Author = "OndFisk",
            SiteTitle = "Opret et Microsoft Teams webinar",
            SiteUrl = "https://docs.microsoft.com/da-dk/dynamics365/marketing/teams-webinar",
            Language = "Danish",
            MaterialType = ResourceType.ARTICLE,
            Categories = new Collection<string>() { },
            Tags = new Collection<string>() { },
        };

        await _repo.Push(mircrosoftResource);

        var actual = await _repo.GetAll();

        var learnItResourceDTO = new ResourceDTO(
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

        var microsoftResourceDTO = new ResourceDTO(
            2,
            ResourceType.ARTICLE,
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

    [Fact]
    public async Task ResourceLixLevel_equals_55_returns_4_skillLevel()
    {
        //await _repo.Push(resource);

        var actual = await (from tag in _context.Tags select tag.Name).ToListAsync();

        var expected = new Collection<string>() {"C#", "Java"};
    }

    [Fact]
    public async Task Push_resource_with_new_tags_creates_new_tags_in_DB()
    {
        var resource = new ResourceCreateDTO()
        {
            Created = Created,
            MaterialType = ResourceType.ARTICLE,
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            Categories = new Collection<string>(),
            Tags = new Collection<string>() {"C#", "Java"}
        };
        await _repo.Push(resource);

        var actual = await _context.Tags.Select(c => c.Name).ToListAsync();

        var expected = new Collection<string>() {"C#", "Java"};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_sets_author_to_unknown_if_author_is_null()
    {
        var resource = new ResourceCreateDTO()
        {
            Created = new DateTime(2021, 11, 30, 13, 11, 11),
            MaterialType = ResourceType.ARTICLE,
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            Categories = new Collection<string>(),
            Tags = new Collection<string>() {"C#", "Java"},
            Author = null
        };

        await _repo.Push(resource);

        var actual = await _repo.Get(1);

        var expected = new ResourceDetailsDTO()
        {
            Id = 1,
            Created = Created,
            MaterialType = ResourceType.ARTICLE,
            SiteTitle = "My first Page",
            SkillLevel = 1,
            ContentSource = "learnit.itu.dk",
            LixNumber = 0,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            Language = "Danish",
            Categories = new Collection<string>(),
            Tags = new Collection<string>() {"C#", "Java"},
            Author = "Unknown",
            Comments = new Collection<string>()
        };

        expected.Should().BeEquivalentTo(actual.Value);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_no_existing_resource()
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
            Tags = new Collection<string>() {"C#"},
            Categories = new Collection<string>() {"Programming"},
            MaterialType = ResourceType.ARTICLE
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
            LixNumber = 11,
            Language = "English",
            MaterialType = ResourceType.ARTICLE,
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
            LixNumber = 11,
            SkillLevel = 1,
            MaterialType = ResourceType.ARTICLE,
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
}