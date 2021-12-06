namespace VideoOverflow.Infrastructure.Tests;

public class UserRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly UserRepository _repo;

    public UserRepositoryTests()
    {
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async Task GetAll_returns_all_users()
    {
        var userDeniz = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>()};
        var userAlex = new UserCreateDTO() {Name = "Alex", Comments = new List<string>()};
        var userKarl = new UserCreateDTO() {Name = "Karl", Comments = new List<string>()};
        var userAsmus = new UserCreateDTO() {Name = "Asmus", Comments = new List<string>()};
        var userAnton = new UserCreateDTO() {Name = "Anton", Comments = new List<string>()};
        var userChristan = new UserCreateDTO() {Name = "Christan", Comments = new List<string>()};

        await _repo.Push(userDeniz);
        await _repo.Push(userAlex);
        await _repo.Push(userKarl);
        await _repo.Push(userAsmus);
        await _repo.Push(userAnton);
        await _repo.Push(userChristan);

        var userDenizDTO = new UserDTO(1, "Deniz", new List<string>());
        var userAlexDTO = new UserDTO(2, "Alex", new List<string>());
        var userKarlDTO = new UserDTO(3, "Karl", new List<string>());
        var userAsmusDTO = new UserDTO(4, "Asmus", new List<string>());
        var userAntonDTO = new UserDTO(5, "Anton", new List<string>());
        var userChristanDTO = new UserDTO(6, "Christan", new List<string>());

        var expected = new List<UserDTO>()
            {userDenizDTO, userAlexDTO, userKarlDTO, userAsmusDTO, userAntonDTO, userChristanDTO};

        var actual = await _repo.GetAll();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_Users()
    {
        var actual = await _repo.GetAll();

        Assert.Empty(actual);
    }
    
    [Fact]
    public async Task GetAll_returns_all_comments_of_User()
    {
        var userDeniz = new UserCreateDTO() {Name = "Deniz", Comments = new Collection<string>(){}};
        
        await _repo.Push(userDeniz);

        var userEntity = await _context.Users.FirstAsync();
        
        userEntity.Comments.Add(new Comment()
        {
            Id = 1,
            AttachedToResource = 0,
            Content = "Hello",
            CreatedBy = 1
            
        });
        userEntity.Comments.Add(new Comment()
        {
            Id = 2,
            AttachedToResource = 0,
            Content = "my second comment",
            CreatedBy = 1
            
        });

        var actual = await _repo.Get(1);
        var expected = new Collection<string>() {"Hello", "my second comment"};

        expected.Should().BeEquivalentTo(actual.Comments);
    }

    [Fact]
    public async Task Push_creates_new_user_with_id_and_given_name()
    {
        var userSødFisk = new UserCreateDTO()
        {
            Name = "SødFisk",
            Comments = new List<string>()
            {
                "Nice video",
                "A simple comment",
                "Very cool project!"
            }
        };

        var actual = await _repo.Push(userSødFisk);

        var expected = new UserDTO(1, "SødFisk",
            new List<string>()
        );

        expected.Should().BeEquivalentTo(actual);
    }


    [Fact]
    public async Task Push_returns_new_userDTO_with_id_name_and_comments()
    {
        var expected = await _repo.Push(new UserCreateDTO()
            {Name = "Deniz", Comments = new Collection<string>() {"my first comment"}});

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }


    [Fact]
    public async Task Get_given_existing_userId_returns_UserDTO()
    {
        var user = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>()};

        await _repo.Push(user);

        var actual = await _repo.Get(1);

        var expected = new UserDTO(1, "Deniz", new List<string>());


        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Get_non_existing_user_returns_null()
    {
        var user = await _repo.Get(10);

        Assert.Null(user);
    }

    [Fact]
    public async Task Update_existing_user_returns_Updated()
    {
        var user = new UserCreateDTO() {Name = "OndFisk", Comments = new List<string>()};

        await _repo.Push(user);

        var updateUser = new UserUpdateDTO() {Id = 1, Name = "SødFisk", Comments = new List<string>()};

        var actual = await _repo.Update(updateUser);

        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_existing_user_changes_name()
    {
        var user = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>() { }};

        await _repo.Push(user);

        var userUpdate = new UserUpdateDTO()
        {
            Id = 1, Name = "Karl",
            Comments = new List<string>() {}
        };

        await _repo.Update(userUpdate);


        var expected = new UserDTO(1, "Karl",
            new List<string>() {});

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Update_nonExisting_user_returns_NotFound()
    {
        var userUpdate = new UserUpdateDTO() {Id = 100, Name = "Ondfisk"};
        var actual = await _repo.Update(userUpdate);

        Assert.Equal(Status.NotFound, actual);
    }
    
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
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}