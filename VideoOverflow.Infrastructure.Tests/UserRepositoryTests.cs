

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;

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
        var user1 = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>(){"Awesome Video!"}};
        var user2 = new UserCreateDTO() {Name = "Alex", Comments = new List<string>(){"I love the guide"}};
        var user3 = new UserCreateDTO() {Name = "Karl", Comments = new List<string>(){"My first ever comment", "My second ever comment", "Maybe my third ever comment"}};
        var user4 = new UserCreateDTO() {Name = "Asmus", Comments = new List<string>()};
        var user5 = new UserCreateDTO() {Name = "Anton", Comments = new List<string>()};
        var user6 = new UserCreateDTO() {Name = "Christan", Comments = new List<string>()};

         _repo.Push(user1);
         _repo.Push(user2);
         _repo.Push(user3);
         _repo.Push(user4);
         _repo.Push(user5);
         _repo.Push(user6);

        var users = await _repo.GetAll();
        
        Assert.Collection(users,
            user => Assert.Equal(new UserDTO(1, "Deniz", new List<string>(){"Awesome video!"}), user),
            user => Assert.Equal(new UserDTO(2, "Alex", null), user),
            user => Assert.Equal(new UserDTO(3, "Karl", new List<string>(){"My first ever comment", "My second ever comment", "Maybe my third ever comment"}), user),
            user => Assert.Equal(new UserDTO(4, "Asmus", new List<string>()), user),
            user => Assert.Equal(new UserDTO(5, "Anton", new List<string>()), user),
            user => Assert.Equal(new UserDTO(6, "Christan", new List<string>()), user));

        
    }

    [Fact]
    public async Task Get_Given_existing_UserID_returns_UserDTO()
    {
        var user = new UserCreateDTO() {Name = "Deniz", Comments = new List<string>() {"awesome video!"}};

        await _repo.Push(user);

        var actual = await _repo.Get(1);

        var expected = new UserDTO(1, "Deniz", new List<string>() {"awesome video!"});

        Assert.Equal(expected.Name, actual.Name);
        
        

    } 
}