namespace VideoOverflow.Infrastructure.Tests;

public class CommentRepositoryTests : RepositoryTestsSetup
{
    private readonly CommentRepository _repo;

    public CommentRepositoryTests()
    {
        _repo = new CommentRepository(_context);
    }

    [Fact]
    public async Task GetAll_returns_all_comments_with_creator()
    {
        var firstUser = new User() {Id = 1, Name = "OndFisk", Comments = new Collection<Comment>()};
        var secondUser = new User() {Id = 2, Name = "SødFisk", Comments = new Collection<Comment>()};


        var firstUserEntry = _context.Users.Add(firstUser);
        var secondUserEntry = _context.Users.Add(secondUser);


        var firstComment = new CommentCreateDTO()
            {CreatedBy = firstUserEntry.Entity.Id, Content = "This docker tutorial is smooth"};
        var secondComment = new CommentCreateDTO()
            {CreatedBy = firstUserEntry.Entity.Id, Content = "Very helpful guide for beginners!"};
        var thirdComment = new CommentCreateDTO() {CreatedBy = secondUserEntry.Entity.Id, Content = "Indeed"};
        var fourthComment = new CommentCreateDTO()
            {CreatedBy = secondUserEntry.Entity.Id, Content = "Thank you very much!"};

        await _repo.Push(firstComment);
        await _repo.Push(secondComment);
        await _repo.Push(thirdComment);
        await _repo.Push(fourthComment);

        var actual = await _repo.GetAll();

        var expected = new Collection<CommentDTO>()
        {
            new(1, 1, 0, "This docker tutorial is smooth"),
            new(2, 1, 0, "Very helpful guide for beginners!"),
            new(3, 2, 0, "Indeed"),
            new(4, 2, 0, "Thank you very much!"),
        };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_returns_empty_list_for_non_existing_comments()
    {
        var actual = await _repo.GetAll();

        var expected = new ReadOnlyCollection<CommentDTO>(new Collection<CommentDTO>());

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_creates_new_comment_with_id_and_content()
    {
        var comment = new CommentCreateDTO() {Content = "Nice Video!"};

        var actual = await _repo.Push(comment);

        Assert.Equal(1, actual.Id);
        Assert.Equal("Nice Video!", actual.Content);
    }

    [Fact]
    public async Task Push_adds_comment_to_resource_and_creates_new_list_of_comments_if_null()
    {
        await _context.Resources.AddAsync(new Resource()
        {
            Id = 1,
            Author = "deniz",
            Categories = new List<Category>(),
            ContentSource = "YouTube",
            Language = "Danish",
            MaterialType = ResourceType.BOOKS,
            SiteTitle = "My First Page",
            SiteUrl = "https://www.mma.com",
            Tags = new List<Tag>(),
            Comments = null
        });

        await _context.SaveChangesAsync();

        var comment = new CommentCreateDTO() {Content = "Nice Book!", AttachedToResource = 1};

        await _repo.Push(comment);

        var actual = await _context.Resources.FindAsync(1);

        var expected = new Resource()
        {
            Id = 1,
            Author = "deniz",
            Categories = new List<Category>(),
            ContentSource = "YouTube",
            Language = "Danish",
            MaterialType = ResourceType.BOOKS,
            SiteTitle = "My First Page",
            SiteUrl = "https://www.mma.com",
            Tags = new List<Tag>(),
            Comments = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    Content = "Nice Book!",
                    AttachedToResource = 1,
                    CreatedBy = 0
                }
            }
        };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_comment_on_resource_adds_comment_to_resource_by_existing_user()
    {
        var resource = new Resource()
        {
            Id = 1,
            Created = Created,
            MaterialType = ResourceType.VIDEO,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            SiteTitle = "My first Page",
            ContentSource = "Youtube",
            LixNumber = 1,
            SkillLevel = 1,
            Author = "Deniz",
            Language = "Danish",
            Tags = new Collection<Tag>() { },
            Categories = new Collection<Category>() { },
            Comments = new Collection<Comment>()
        };


        var user = new User() {Name = "Deniz", Comments = new Collection<Comment>()};

        var comment = new CommentCreateDTO()
            {Content = "I just commented on my own post :-)", AttachedToResource = 1, CreatedBy = 1};

        await _context.Users.AddAsync(user);
        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();
        await _repo.Push(comment);

        var expected = await _context.Resources.FindAsync(1);

        var actual = await _context.Resources.Where(c => c.Id == expected.Id).FirstOrDefaultAsync();

        expected.Should().BeEquivalentTo(actual);
    }

    
    [Fact]
    public async Task Update_comment_updates_user_comments_and_resource_comments()
    {
        var resource = new Resource()
        {
            Id = 1,
            Created = Created,
            MaterialType = ResourceType.VIDEO,
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            SiteTitle = "My first Page",
            ContentSource = "Youtube",
            LixNumber = 1,
            SkillLevel = 1,
            Author = "Deniz",
            Language = "Danish",
            Tags = new Collection<Tag>() { },
            Categories = new Collection<Category>() { },
            Comments = new Collection<Comment>()
        };
        
        var user = new User() {Name = "Deniz", Comments = new Collection<Comment>()};

        var comment = new CommentCreateDTO()
            {Content = "My first ever post", AttachedToResource = 1, CreatedBy = 1};

        await _context.Users.AddAsync(user);
        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();
        await _repo.Push(comment);

        var updateComment = new CommentUpdateDTO()
        {
            AttachedToResource = 1, CreatedBy = 1, Content = "I also changed in resource comments and user comments", Id = 1
        };

        await _repo.Update(updateComment);
        
        var expectedResourceComments = (await _context.Resources.FirstAsync()).Comments;
        var expectedUserComments = (await _context.Users.FirstAsync()).Comments;

        var actualResourceComments = new Collection<Comment>()
        {
            new Comment()
            {
                Content = "I also changed in resource comments and user comments",
                CreatedBy = 1,
                AttachedToResource = 1,
                Id = 1
            }
        };
        var actualUserComments = new Collection<Comment>()
        {
            new Comment()
            {
                Content = "I also changed in resource comments and user comments",
                CreatedBy = 1,
                AttachedToResource = 1,
                Id = 1
            }
        };

        expectedResourceComments.Should().BeEquivalentTo(actualResourceComments);
        expectedUserComments.Should().BeEquivalentTo(actualUserComments);
        
    }

    [Fact]
    public async Task Get_returns_comment_for_given_id()
    {
        var category = new CommentCreateDTO() {Content = "A simple comment"};

        await _repo.Push(category);

        var expected = new CommentDTO(1, 0, 0, "A simple comment");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Get_returns_null_for_non_existing_comment()
    {
        var comment = await _repo.Get(4);

        Assert.Null(comment);
    }

    [Fact]
    public async Task Get_returns_creator()
    {
        var user = new User() {Id = 1, Name = "SødFisk", Comments = new List<Comment>()};

        await _context.Users.AddAsync(user);

        var comment = new CommentCreateDTO() {CreatedBy = 1, Content = "This comment is created by SødFisk"};

        await _repo.Push(comment);

        var actual = new User();


        foreach (var commenter in _context.Users)
        {
            if (commenter.Id == comment.CreatedBy)
            {
                actual = commenter;
            }
        }

        Assert.Equal(actual, user);
    }


    [Fact]
    public async Task Update_of_existing_comment_returns_Updated()
    {
        var comment = new CommentCreateDTO() {Content = "This is an awesome project!"};

        await _repo.Push(comment);

        var update = new CommentUpdateDTO() {Id = 1, Content = "Yes, i mean it's crazy!"};

        var actual = await _repo.Update(update);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_comment()
    {
        var update = new CommentUpdateDTO() {Id = 10, Content = "Am i trying to change an non-existing comment?"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task Update_changes_content_of_comment()
    {
        var comment = new CommentCreateDTO() {Content = "Awesome tutorial!"};

        await _repo.Push(comment);

        var update = new CommentUpdateDTO() {Id = 1, Content = "Maybe it wasn't that good"};

        await _repo.Update(update);

        var expected = new CommentDTO(1, 0, 0, "Maybe it wasn't that good");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }
}