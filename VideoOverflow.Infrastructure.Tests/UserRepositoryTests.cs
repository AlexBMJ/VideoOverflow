namespace VideoOverflow.Infrastructure.Tests;

/// <summary>
/// Tests for our userRepository
/// </summary>
public class UserRepositoryTests : RepositoryTestsSetup, IDisposable
{
    private readonly UserRepository _repo;

    /// <summary>
    /// Instantiate each test with a fresh repository
    /// </summary>
    public UserRepositoryTests()
    {
        _repo = new UserRepository(_context);
    }

    /// <summary>
    /// Test the userRepository's getAll method
    /// to ensure it returns all users
    /// </summary>
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

    /// <summary>
    /// Test the userRepository's getAll method for an empty DB
    /// to ensure it returns an empty collection
    /// </summary>
    [Fact]
    public async Task GetAll_returns_empty_list_for_no_existing_Users()
    {
        var actual = await _repo.GetAll();

        Assert.Empty(actual);
    }
    
    /// <summary>
    /// Test the userRepository's getAll method with comments added to users
    /// to ensure that all users have the correct comments paired with them
    /// </summary>
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

    /// <summary>
    /// Test the userRepository's push method
    /// to ensure it creates a new user in the DB
    /// </summary>
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

    /// <summary>
    /// Test the userRepository's push method
    /// to ensure it creates the user in the DB
    /// </summary>
    [Fact]
    public async Task Push_returns_new_userDTO_with_id_name_and_comments()
    {
        var expected = await _repo.Push(new UserCreateDTO()
            {Name = "Deniz", Comments = new Collection<string>() {"my first comment"}});

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the userRepository's get method for an existing user
    /// to ensure it returns the correct user
    /// </summary>
    [Fact]
    public async Task Get_given_existing_userId_returns_UserDTO()
    {
        var user = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>()};

        await _repo.Push(user);

        var actual = await _repo.Get(1);

        var expected = new UserDTO(1, "Deniz", new List<string>());


        expected.Should().BeEquivalentTo(actual);
    }

    /// <summary>
    /// Test the userRepository's get method for a non existing user
    /// to ensure it returns null
    /// </summary>
    [Fact]
    public async Task Get_non_existing_user_returns_null()
    {
        var user = await _repo.Get(10);

        Assert.Null(user);
    }

    /// <summary>
    /// Test the userRepository's update method for an existing user
    /// to ensure the updated action result is returned
    /// </summary>
    [Fact]
    public async Task Update_existing_user_returns_Updated()
    {
        var user = new UserCreateDTO() {Name = "OndFisk", Comments = new List<string>()};

        await _repo.Push(user);

        var updateUser = new UserUpdateDTO() {Id = 1, Name = "SødFisk", Comments = new List<string>()};

        var actual = await _repo.Update(updateUser);

        Assert.Equal(Status.Updated, actual);
    }

    /// <summary>
    /// Test the userRepository's update method for an existing user
    /// to ensure it updates the correct user in the DB
    /// </summary>
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

    /// <summary>
    /// Test the userRepository's update method on a non existing user
    /// to ensure it returns the actionresult notFound
    /// </summary>
    [Fact]
    public async Task Update_nonExisting_user_returns_NotFound()
    {
        var userUpdate = new UserUpdateDTO() {Id = 100, Name = "Ondfisk"};
        var actual = await _repo.Update(userUpdate);

        Assert.Equal(Status.NotFound, actual);
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