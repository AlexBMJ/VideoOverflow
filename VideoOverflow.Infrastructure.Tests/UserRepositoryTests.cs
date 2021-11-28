namespace VideoOverflow.Infrastructure.Tests;

public class UserRepositoryTests
{

    private readonly UserRepository _repo;
    private readonly VideoOverflowContext _context;
    
    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();

        context.SaveChanges();
        
        _context = context;
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async Task GetAll_returns_all_users()
    {
        var user1 = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>()};
        var user2 = new UserCreateDTO() {Name = "Alex", Comments = new List<string>()};
        var user3 = new UserCreateDTO() {Name = "Karl", Comments = new List<string>()};
        var user4 = new UserCreateDTO() {Name = "Asmus", Comments = new List<string>()};
        var user5 = new UserCreateDTO() {Name = "Anton", Comments = new List<string>()};
        var user6 = new UserCreateDTO() {Name = "Christan", Comments = new List<string>()};

        await _repo.Push(user1);
        await _repo.Push(user2);
        await _repo.Push(user3);
        await _repo.Push(user4);
        await _repo.Push(user5);
        await _repo.Push(user6);
         
         var userdto1 = new UserDTO(1, "Deniz", new List<string>());
         var userdto2 = new UserDTO(2, "Alex", new List<string>());
         var userdto3 = new UserDTO(3, "Karl", new List<string>());
         var userdto4 = new UserDTO(4, "Asmus", new List<string>());
         var userdto5 = new UserDTO(5, "Anton", new List<string>());
         var userdto6 = new UserDTO(6, "Christan", new List<string>());

         var expected = new List<UserDTO>() {userdto1, userdto2, userdto3, userdto4, userdto5, userdto6};
        
         var users = await _repo.GetAll();

         for (int i = 0; i < users.Count; i++)
         {
             expected.GetEnumerator().Current.Should().BeEquivalentTo(users.GetEnumerator().Current);
             expected.GetEnumerator().MoveNext();
             users.GetEnumerator().MoveNext();
         }
    }
    
    [Fact]
    public async Task GetAll_Returns_Empty_List_For_No_existsing_Users()
    {
        var actual = await _repo.GetAll();

        var expected = new ReadOnlyCollection<UserDTO>(new Collection<UserDTO>());

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Push_creates_new_User_with_autogenerated_id_and_given_name()
    {
        var user = new UserCreateDTO() {Name = "SødFisk", 
            Comments = new List<string>()
        {
            "Nice video",
            "A simple comment",
            "Very cool project!"
        }};

        var actual = await _repo.Push(user);

        var expected = new UserDTO(1, "SødFisk", 
            new List<string>()
           );


        expected.Should().BeEquivalentTo(actual);
    }
    

    [Fact]
    public async Task Get_Given_existing_UserID_returns_UserDTO()
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
    public async Task Update_Existing_User_Returns_StatusUpdated()
    {
        var user = new UserCreateDTO() {Name = "OndFisk", Comments = new List<string>()};

        await _repo.Push(user);

        var updateUser = new UserUpdateDTO() {Id = 1, Name = "SødFisk", Comments = new List<string>()};

        var actual = await _repo.Update(updateUser);
        
        Assert.Equal(Status.Updated, actual);
    }

    [Fact]
    public async Task Update_Existing_User_Changes_Comments_and_Name()
    {
        var user = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>() {}};

        await _repo.Push(user);

        var userUpdate = new UserUpdateDTO()
            {Id = 1, Name = "Karl", Comments = new List<string>() {"I changed name from Deniz to Karl and added my first comment"}};

        await _repo.Update(userUpdate);
        

        var expected = new UserDTO(1, "Karl", new List<string>() {"I changed name from Deniz to Karl and added my first comment"});

        var actual = await _repo.Get(1);

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task Update_NonExisting_User_Returns_StatusNotFound()
    {
        var userUpdate = new UserUpdateDTO() {Id = 100, Name = "Ondfisk"};
        var actual = await _repo.Update(userUpdate);
        
        Assert.Equal(Status.NotFound, actual);
    }
}