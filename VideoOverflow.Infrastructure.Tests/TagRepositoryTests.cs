using Moq;

namespace VideoOverflow.Infrastructure.Tests;

public class TagRepositoryTests
{
    private readonly VideoOverflowContext _context;
    private readonly TagRepository _repo;

    public TagRepositoryTests()
    {
        var repo = new RepositoryTestsSetup();
        _context = repo.Context;

        _repo = new TagRepository(_context);
    }

    //[Fact]
    public async Task GetAll_Returns_All_MockTags()
    {
        
        // Look into this.. gives error
        
        var moqTag1 = new Mock<TagCreateDTO>();
        moqTag1.SetupAllProperties();
        
        
        moqTag1.Setup(c => c.Name).Returns("CSharp");
        moqTag1.Setup(c => c.TagSynonyms).Returns(new List<string>(){"CS", "C-sharp", "c#"});

        var moqTag2 = new Mock<TagCreateDTO>();
        moqTag2.SetupAllProperties();
        moqTag2.Setup(c => c.Name).Returns("Java");
        moqTag2.Setup(c => c.TagSynonyms).Returns(new List<string>() {"jav", "javaa", "javaaa"});

        var moqTag3 = new Mock<TagCreateDTO>();
        moqTag2.SetupAllProperties();
        moqTag2.Setup(c => c.Name).Returns("Docker");
        moqTag2.Setup(c => c.TagSynonyms).Returns(new List<string>() {"Dock", "DC", "Just Testing"});

/*
        await _repo.Push(moqTag1);
        await _repo.Push(moqTag2);
        await _repo.Push(moqTag3);
*/
        
        
        var moqTagDTO1 = new TagDTO(1, "CSharp",
            new List<string>()
            {
                "CS", "c#", "c-sharp"
            });
        
        var moqTagDTO2 = new TagDTO(2, "Java",
            new List<string>()
            {
                "jav", "javaa", "javaaa"
            });
        
        var moqTagDTO3 = new TagDTO(3, "Docker",
            new List<string>()
            {
                "Dock", "DC", "Just Testing"
            });

        var actual = await _repo.GetAll();

        var expected = new Collection<TagDTO>() {moqTagDTO1, moqTagDTO2, moqTagDTO3};

        expected.Should().BeEquivalentTo(actual);


    }

    [Fact]
    public async Task GetAll_Returns_All_Tags()
    {


        var tag1 = new TagCreateDTO()
        {
            Name = "CSharp",
            TagSynonyms = new List<string>()
            {
                "CS", "c#", "c-sharp"
            }
        };
        
        var tag2 = new TagCreateDTO()
        {
            Name = "Java",
            TagSynonyms = new List<string>()
            {
                "jav", "javaa", "javaaa"
            }
        };
        
        var tag3 = new TagCreateDTO()
        {
            Name = "Docker",
            TagSynonyms = new List<string>()
            {
                "Dock", "DC", "Just Testing"
            }
        };
        

        await _repo.Push(tag1);
        await _repo.Push(tag2);
        await _repo.Push(tag3);
        
            
            
            
        var tagDTO1 = new TagDTO(1, "CSharp",
            new List<string>()
            {
                "CS", "c#", "c-sharp"
            });
        
        var tagDTO2 = new TagDTO(2, "Java",
            new List<string>()
            {
                "jav", "javaa", "javaaa"
            });
        
        var tagDTO3 = new TagDTO(3, "Docker",
            new List<string>()
            {
                "Dock", "DC", "Just Testing"
            });

        var actual = await _repo.GetAll();

        var expected = new Collection<TagDTO>() {tagDTO1, tagDTO2, tagDTO3};

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_Empty_Table()
    {
        var actual = await _repo.GetAll();

        var expected = new Collection<TagDTO>();

        expected.Should().BeEquivalentTo(actual);
    }
    
    [Fact]
    public async Task Get_Existing_Tag_Returns_TagDTO_for_given_id()
    {
        var tag1 = new TagCreateDTO()
        {
            Name = "Python",
            TagSynonyms = new List<string>()
            {
                "PY",
                "PYTH",
                "P3"
            }
        };
        
        var tag2 = new TagCreateDTO()
        {
            Name = "Pyton",
            TagSynonyms = new List<string>()
            {
                "PY",
                "PYTH",
                "P3"
            }
        };

        var firstTagPushed = await _repo.Push(tag1);
        var actual = await _repo.Push(tag2);
        

        var expected = new TagDTO(2,
            "Pyton",
            new List<string>()
            {
                "PY",
                "PYTH",
                "P3"
            });

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_Non_Existing_Tag_Returns_null()
    {
        var actual = await _repo.Get(1000);
        
        Assert.Null(actual);
    }

    [Fact]
    public async Task Push_Creates_Id_In_DB_and_returns_TagDTO_for_created_entity()
    {
        var tag = new TagCreateDTO()
        {
            Name = "C#",
            TagSynonyms = new List<string>()
            {
                "Csharp",
                "c-sharp",
                "c#"
            }
        };
        
        var tag2 = new TagCreateDTO()
        {
            Name = "Python",
            TagSynonyms = new List<string>()
            {
                "PY"
            }
        };

        var actual = await _repo.Push(tag);
        var actual2 = await _repo.Push(tag2);

        var expected = new TagDTO(1, "C#", new List<string>()
        {
            "Csharp",
            "c-sharp",
            "c#"
        });

        var expected2 = new TagDTO(2, "Python", new List<string>()
        {
            "PY"
        });

        expected.Should().BeEquivalentTo(actual);
        expected2.Should().BeEquivalentTo(actual2);
    }
    
    [Fact]
    public async Task Update_Non_Existing_Tag_Returns_StatusNotFound()
    {
        var update = new TagUpdateDTO() {Id = 2, Name = "I'm trying to do something illegal"};
        var actual = await _repo.Update(update);
        
        Assert.Equal(Status.NotFound, actual);
    }
    
    
    [Fact]
    public async Task Update_Existing_Tag_Returns_StatusUpdated()
    {
        var tag = new TagCreateDTO()
        {
            Name = "Csharp",
            TagSynonyms = new List<string>()
            {
                "C#",
                "C-sharp",
                "CS"
            }
        };

        await _repo.Push(tag);
        
        var update = new TagUpdateDTO()
        {
            Id = 1, 
            Name = "I'm changing name from Csharp to Java",
            TagSynonyms = new List<string>()
            {
                "J",
                "test"
            }
            
        };
        
        var actual = await _repo.Update(update);
        
        Assert.Equal(Status.Updated, actual);
    }
    
    [Fact] 
    public async Task Update_changes_content_of_comment_of_givenID()
    {
        var tag = new TagCreateDTO() {Name = "C#"};

        await _repo.Push(tag);

        var update = new TagUpdateDTO() {Id = 1, Name = "Java"};

        await _repo.Update(update);

        var expected = new TagDTO(1, "Java", new List<string>());

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }
    
}