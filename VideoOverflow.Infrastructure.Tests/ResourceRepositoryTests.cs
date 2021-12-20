
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VideoOverflow.Infrastructure.Tests;

public class ResourceRepositoryTests : DatabaseTestCase, IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ResourceRepository _repo;
    private readonly ResourceRepository _pgRepo;
    private readonly VideoOverflowContext _context;
    private readonly ResourceCreateDTO _resource;
    protected readonly DateTime Created = DateTime.Parse("2020-09-29");
    public ResourceRepositoryTests(DatabaseTemplateFixture databaseFixture, ITestOutputHelper testOutputHelper) : base(databaseFixture) {
        _testOutputHelper = testOutputHelper;
        _pgRepo = new ResourceRepository(_pgContext);
        
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();
        context.SaveChanges();
        _context = context;
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
            Created,
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

        var update = new ResourceUpdateDTO
        {
            Id = 1,
            Created = _resource.Created,
            Author = _resource.Author,
            SiteTitle = _resource.SiteTitle,
            SiteUrl = _resource.SiteUrl,
            Language = _resource.Language,
            MaterialType = _resource.MaterialType,
            Categories = _resource.Categories,
            Tags = _resource.Tags,
            Comments = new List<string>() {comment.Content}
        };
        
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var updateResponse = _repo.Update(update);

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
            Created,
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
            Created,
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
        var resource = new ResourceCreateDTO
        {
            Created = _resource.Created,
            Author = _resource.Author,
            SiteTitle = _resource.SiteTitle,
            SiteUrl = _resource.SiteUrl,
            Language = _resource.Language,
            MaterialType = _resource.MaterialType,
            Categories = _resource.Categories,
            Tags = _resource.Tags,
            LixNumber = lixLevel
        };
        await _repo.Push(resource);

        var actual =  _repo.Get(1).Result.Value.SkillLevel;
        var expected = expectedSkillLevel;

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

        var actualObject = await _repo.Get(1);
        
        Assert.Equal(0, actualObject.Value.LixNumber);
        Assert.Equal(1, actualObject.Value.SkillLevel);
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

    [Fact]
    public async Task Delete_returns_NotFound_for_no_existing_resource()
    {
        var actual = await _repo.Delete(100);

        Assert.Equal(Status.NotFound, actual);
    }
    
    [Fact]
    public async Task Delete_returns_Deleted_for_existing_resource()
    {
        await _repo.Push(_resource);
        var actual = await _repo.Delete(1);

        Assert.Equal(Status.Deleted, actual);
    }
    
    [Fact]
    public async Task Delete_actually_deletes_resource()
    {
        await _repo.Push(_resource);
        await _repo.Delete(1);

        var instance = await _repo.Get(1);

        Assert.True(instance.IsNone);
    }
    
    [Fact]
    public async Task Delete_deletes_comments_attachedToResource()
    {
        var comment = _context.Comments.Add(new Comment() {AttachedToResource = 1, Content = "hello", CreatedBy = 0}).Entity;

        await _context.Resources.AddAsync(new Resource()
        {
            Created = Created,
            Author = "Deniz",
            SiteTitle = "My first Page",
            LixNumber = -121212,
            SiteUrl = "ThisIsAnInvalidURL.com",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            ContentSource = "thisis.com",
            Categories = new Collection<Category>(),
            Tags = new Collection<Tag>(),
            Comments = new List<Comment>() {comment}
        });
        
        await _context.SaveChangesAsync();
        
        await _repo.Delete(1);

        _context.Comments.Should().BeEmpty();
    }
    [Fact]
    public async Task comments_attachedToResource_are_attached()
    {
        await _context.Resources.AddAsync(new Resource()
        {
            Created = Created,
            Author = "author",
            SiteTitle = "title",
            LixNumber = 1,
            SiteUrl = "https://www.example.com",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            ContentSource = "example.com",
            Categories = new Collection<Category>(),
            Tags = new Collection<Tag>(),
            Comments = new List<Comment>() 
        });
        await _context.Resources.AddAsync(new Resource()
        {
            Created = Created,
            Author = "author2",
            SiteTitle = "title2",
            LixNumber = 2,
            SiteUrl = "https://www.example2.com",
            Language = "Danish",
            MaterialType = ResourceType.Video,
            ContentSource = "example2.com",
            Categories = new Collection<Category>(),
            Tags = new Collection<Tag>(),
            Comments = new List<Comment>() 
        });
        var commentHello = _context.Comments.Add(new Comment() {AttachedToResource = 1, Content = "hello", CreatedBy = 0}).Entity;
        var commentNice = _context.Comments.Add(new Comment() {AttachedToResource = 1, Content = "nice", CreatedBy = 0}).Entity;
        var commentGreat = _context.Comments.Add(new Comment() {AttachedToResource = 2, Content = "great", CreatedBy = 0}).Entity;
        await _context.SaveChangesAsync();

        var expected = new List<Comment>() {commentHello, commentNice};
        var expected2 = new List<Comment>() {commentGreat};
        GetComments(1).Should().Equal(expected);
        GetComments(2).Should().Equal(expected2);
    }
    private List<Comment> GetComments(int resourceId)
    {
        var collection = new List<Comment>();

        foreach (var comment in _context.Comments)
        {
            if (comment.AttachedToResource == resourceId)
            {
                collection.Add(comment);
            }
        }
        return collection;
    }
[Fact]
    public async Task Get_Resources_returns_last_3_resources_with_tag_a_and_b_given_13_resources_with_tag_a_and_b_and_others_and_no_category_and_skip_10_and_count_10_and_tags_a_and_b()
    {
        await _context.Tags.AddAsync(new Tag()
            {Id = 1, Name = "aaaaaaaaaaaaaaa", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.Tags.AddAsync(new Tag()
            {Id = 2, Name = "bbbbbbbbbbbbbb", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.Tags.AddAsync(new Tag()
            {Id = 3, Name = "ccccccccccccccccccccccccccc", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.SaveChangesAsync();
        
        var resources = new List<ResourceCreateDTO>()
        {
            
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title1",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title2",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title3",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title4",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title5",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title6",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title7",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title8",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title9",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title10",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title11",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title12",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title13",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title14",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
        };
        var created = new List<string>();
        foreach (var createDto in resources)
        {
            created.Add((await _pgRepo.Push(createDto)).SiteTitle);
        }

        
        var expected = created.GetRange(10,3);
        var tags = new List<TagDTO>() {new TagDTO(1, "aaaaaaaaaaaaaaa", new List<string>()), new TagDTO(2, "bbbbbbbbbbbbbb", new List<string>())};
        List<ResourceDTO> actual = (List<ResourceDTO>) await _pgRepo.GetResources(0, "title", tags, 10, 10);
        
        actual.Should().HaveSameCount(expected);
        expected.Should().BeEquivalentTo(actual.Select(a => a.SiteTitle));
        
        
        
    }
    [Fact]
    public async Task Get_Resources_returns_first_10_resources_with_tag_a_and_b_given_13_resources_with_tag_a_and_b_and_others_and_no_category_and_skip_0_and_count_10_and_tags_a_and_b()
    {
        await _context.Tags.AddAsync(new Tag()
            {Id = 1, Name = "aaaaaaaaaaaaaaa", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.Tags.AddAsync(new Tag()
            {Id = 2, Name = "bbbbbbbbbbbbbb", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.Tags.AddAsync(new Tag()
            {Id = 3, Name = "ccccccccccccccccccccccccccc", Resources = new List<Resource>(), TagSynonyms = new List<TagSynonym>()});
        await _context.SaveChangesAsync();
        
        var resources = new List<ResourceCreateDTO>()
        {
            
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title1",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title2",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title3",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title4",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title5",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title6",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title7",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title8",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title9",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title10",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"aaaaaaaaaaaaaaa", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title11",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb", "ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title12",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title13",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"bbbbbbbbbbbbbb"},
                Categories = new List<string>()
            },
            new()
            {
                Created = DateTime.MinValue,
                MaterialType = ResourceType.Article, 
                SiteUrl = "https://a.dk",
                SiteTitle = "title14",
                Author = "mememememe",
                LixNumber = 1,
                Language = "Danish",
                Tags = new List<string>() {"ccccccccccccccccccccccccccc"},
                Categories = new List<string>()
            },
        };
        var created = new List<string>();
        foreach (var createDto in resources)
        {
            created.Add((await _pgRepo.Push(createDto)).SiteTitle);
        }

        
        var expected = created.GetRange(1,9);
        expected.Add(created[13]);
        var tags = new List<TagDTO>() {new TagDTO(1, "aaaaaaaaaaaaaaa", new List<string>()), new TagDTO(2, "bbbbbbbbbbbbbb", new List<string>())};
        List<ResourceDTO> actual = (List<ResourceDTO>) await _pgRepo.GetResources(0, "title", tags, 10, 0);
        actual.Should().HaveSameCount(expected);
        expected.Should().BeEquivalentTo(actual.Select(a => a.SiteTitle));
        
        
        
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