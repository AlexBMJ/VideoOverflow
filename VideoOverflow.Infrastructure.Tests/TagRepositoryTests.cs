namespace VideoOverflow.Infrastructure.Tests;

public class TagRepositoryTests : RepositoryTestsSetup
{
    private readonly TagRepository _repo;

    public TagRepositoryTests()
    {
        _repo = new TagRepository(_context);
    }

    [Fact]
    public async Task GetAll_returns_all_tags()
    {
        var cSharpTag = new TagCreateDTO()
        {
            Name = "CSharp",
            TagSynonyms = new List<string>()
            {
                "CS", "c#", "c-sharp"
            }
        };

        var javaTag = new TagCreateDTO()
        {
            Name = "Java",
            TagSynonyms = new List<string>()
            {
                "jav", "javaa", "javaaa"
            }
        };

        var dockerTag = new TagCreateDTO()
        {
            Name = "Docker",
            TagSynonyms = new List<string>()
            {
                "Dock", "DC", "Just Testing"
            }
        };

        await _repo.Push(cSharpTag);
        await _repo.Push(javaTag);
        await _repo.Push(dockerTag);

        var cSharpDTO = new TagDTO(1, "CSharp",
            new List<string>()
            {
                "CS", "c#", "c-sharp"
            });

        var javaDTO = new TagDTO(2, "Java",
            new List<string>()
            {
                "jav", "javaa", "javaaa"
            });

        var dockerDTO = new TagDTO(3, "Docker",
            new List<string>()
            {
                "Dock", "DC", "Just Testing"
            });

        var actual = await _repo.GetAll();

        var expected = new Collection<TagDTO>() {cSharpDTO, javaDTO, dockerDTO};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_returns_empty_for_no_tags_in_DB()
    {
        var actual = await _repo.GetAll();

        new Collection<TagDTO>().Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_existing_tag_returns_TagDTO()
    {
        var pythonTag = new TagCreateDTO()
        {
            Name = "Python",
            TagSynonyms = new List<string>()
            {
                "PY",
                "PYTH",
                "P3"
            }
        };

        var actual = await _repo.Push(pythonTag);

        var expected = new TagDTO(1,
            "Python",
            new List<string>()
            {
                "PY",
                "PYTH",
                "P3"
            });

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_non_existing_tag_returns_null()
    {
        Assert.Null(await _repo.Get(1000));
    }

    [Fact]
    public async Task Push_returns_TagDTO()
    {
        var cSharpTag = new TagCreateDTO()
        {
            Name = "C#",
            TagSynonyms = new List<string>()
            {
                "Csharp",
                "c-sharp",
                "c#"
            }
        };

        var actual = await _repo.Push(cSharpTag);

        var expected = new TagDTO(1, "C#", new List<string>()
        {
            "Csharp",
            "c-sharp",
            "c#"
        });

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Update_non_existing_Tag_returns_NotFound()
    {
        var actual = await _repo.Update(new TagUpdateDTO() {Id = 2, Name = "I'm trying to do something illegal"});

        Assert.Equal(Status.NotFound, actual);
    }
    
    [Fact]
    public async Task Update_existing_tag_returns_Updated()
    {
        var cSharpTag = new TagCreateDTO()
        {
            Name = "Csharp",
            TagSynonyms = new List<string>()
            {
                "C#",
                "C-sharp",
                "CS"
            }
        };

        await _repo.Push(cSharpTag);

        var updateCSharpTag = new TagUpdateDTO()
        {
            Id = 1,
            Name = "I'm changing name from Csharp to Java",
            TagSynonyms = new List<string>()
            {
                "Jav4",
                "Jav"
            }
        };

        var actual = await _repo.Update(updateCSharpTag);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_changes_name_of_tag()
    {
        var tagCreate = new TagCreateDTO() {Name = "C#", TagSynonyms = new List<string>()};

        await _repo.Push(tagCreate);

        var tagUpdate = new TagUpdateDTO() {Id = 1, Name = "Java", TagSynonyms = new List<string>()};

        await _repo.Update(tagUpdate);

        var expected = new TagDTO(1, "Java", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }
}