namespace VideoOverflow.Infrastructure.Tests;

/// <summary>
/// Tests for our commentRepository
/// </summary>
public class CommentRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly CommentRepository _repo;
    
    /// <summary>
    /// Instantiate each test with a fresh repository
    /// </summary>
    public CommentRepositoryTests()
    {
        _repo = new CommentRepository(_context);
    }

    /// <summary>
    /// Test the commentRepository's getAll method for a non empty DB
    /// to ensure all comments from DB are returned
    /// </summary>
    [Fact]
    public async Task GetAll_returns_all_comments_with_creator()
    {
        var firstUser = new User() {Id = 1, Name = "OndFisk", Comments = new Collection<Comment>()};
        var secondUser = new User() {Id = 2, Name = "SødFisk", Comments = new Collection<Comment>()};


        var firstUserEntry = _context.Users.Add(firstUser);
        var secondUserEntry = _context.Users.Add(secondUser);


        var dockerComment = new CommentCreateDTO() {CreatedBy = firstUserEntry.Entity.Id, Content = "This docker tutorial is smooth"};
        var beginnerComment = new CommentCreateDTO() {CreatedBy = firstUserEntry.Entity.Id, Content = "Very helpful guide for beginners!"};
        var responseComment = new CommentCreateDTO() {CreatedBy = secondUserEntry.Entity.Id, Content = "Indeed"};
        var greetComment = new CommentCreateDTO() {CreatedBy = secondUserEntry.Entity.Id, Content = "Thank you very much!"};

        await _repo.Push(dockerComment);
        await _repo.Push(beginnerComment);
        await _repo.Push(responseComment);
        await _repo.Push(greetComment);

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

    /// <summary>
    /// Test the commentRepository's getAll method for an empty DB
    /// to ensure an empty collection is returned
    /// </summary>
    [Fact]
    public async Task GetAll_returns_empty_list_for_non_existing_comments()
    {
        var actual = await _repo.GetAll();

        var expected = new ReadOnlyCollection<CommentDTO>(new Collection<CommentDTO>());

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the commentRepository's push method
    /// to ensure it gets created correctly
    /// </summary>
    [Fact]
    public async Task Push_creates_new_comment_with_id_and_content()
    {
        var comment = new CommentCreateDTO() {Content = "Nice Video!"};

        var actual = await _repo.Push(comment);

        Assert.Equal(1, actual.Id);
        Assert.Equal("Nice Video!", actual.Content);
    }

    /// <summary>
    /// Test the commentRepository's push method for DB with existing resource
    /// to ensure the comment is added to the resource
    /// </summary>
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
            MaterialType = ResourceType.Book,
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
            MaterialType = ResourceType.Book,
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

    /// <summary>
    /// Test the commentRepository's push method with existing resource and user in the DB
    /// to ensure comment is attached to the resource and user
    /// </summary>
    [Fact]
    public async Task Push_comment_on_resource_adds_comment_to_resource_by_existing_user()
    {
        var resource = new Resource()
        {
            Id = 1,
            Created = Created,
            MaterialType = ResourceType.Video,
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

        var comment = new CommentCreateDTO() {Content = "I just commented on my own post :-)", AttachedToResource = 1, CreatedBy = 1};

        await _context.Users.AddAsync(user);
        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();
        await _repo.Push(comment);

        var expected = await _context.Resources.FindAsync(1);

        var actual = await _context.Resources.Where(c => c.Id == expected.Id).FirstOrDefaultAsync();

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the commentRepository's update method for an existing comment
    /// to ensure the comment is updated accordingly in the DB
    /// </summary>
    [Fact]
    public async Task Update_comment_updates_user_comments_and_resource_comments()
    {
        var resource = new Resource()
        {
            Id = 1,
            Created = Created,
            MaterialType = ResourceType.Video,
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
        
        var actualResourceComments = (await _context.Resources.FirstAsync()).Comments;
        var actualUserComments = (await _context.Users.FirstAsync()).Comments;

        var expectedResourceComments = new Collection<Comment>()
        {
            new Comment()
            {
                Content = "I also changed in resource comments and user comments",
                CreatedBy = 1,
                AttachedToResource = 1,
                Id = 1
            }
        };
        var expectedUserComments = new Collection<Comment>()
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

    /// <summary>
    /// Test the commentRepository's get method for an existing comment
    /// to ensure the correct comment is returned from the DB
    /// </summary>
    [Fact]
    public async Task Get_returns_comment_for_given_id()
    {
        var category = new CommentCreateDTO() {Content = "A simple comment"};

        await _repo.Push(category);

        var expected = new CommentDTO(1, 0, 0, "A simple comment");

        var actual = await _repo.Get(1);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Test the commentRepository's get method for a non existing comment
    /// to ensure null is returned
    /// </summary>
    [Fact]
    public async Task Get_returns_null_for_non_existing_comment()
    {
        var comment = await _repo.Get(4);

        Assert.Null(comment);
    }
    
    [Fact]
    public async Task Get_returns_creator()
    {
        var userCreate = new UserCreateDTO() {Name = "SødFisk", Comments = new List<string>()};
        var userRepo = new UserRepository(_context);
        var user = userRepo.Push(userCreate);

        var commentCreate = new CommentCreateDTO() {CreatedBy = user.Id, Content = "This comment is created by SødFisk"};

        var commentDTO = await _repo.Push(commentCreate);

        var comment = await _repo.Get(commentDTO.Id);

        Assert.Equal(user.Id, comment.CreatedBy);
    }

    /// <summary>
    /// Test the commentRepository's update method for existing comment
    /// to ensure the updated action result is returned
    /// </summary>
    [Fact]
    public async Task Update_of_existing_comment_returns_Updated()
    {
        var comment = new CommentCreateDTO() {Content = "This is an awesome project!"};

        await _repo.Push(comment);

        var update = new CommentUpdateDTO() {Id = 1, Content = "Yes, i mean it's crazy!"};

        var actual = await _repo.Update(update);

        Assert.Equal(Status.Updated, actual);
    }

    /// <summary>
    /// Test the commentRepository's update method for a non existing comment
    /// to ensure the notfound action result is returned
    /// </summary>
    [Fact]
    public async Task Update_returns_NotFound_for_non_existing_comment()
    {
        var update = new CommentUpdateDTO() {Id = 10, Content = "Am i trying to change an non-existing comment?"};

        var response = await _repo.Update(update);

        Assert.Equal(Status.NotFound, response);
    }

    /// <summary>
    /// Test the commentRepository's update method for an existing comment
    /// to ensure the comment is updated accordingly in the DB
    /// </summary>
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
    
    /* Dispose code has been taken from  https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CityRepositoryTests.cs*/
    private bool _disposed;
    /// <summary>
    /// If we haven't disposed yet, we start disposing our context
    /// </summary>
    /// <param name="disposing"> Boolean for if we have disposed yet</param>
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
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}