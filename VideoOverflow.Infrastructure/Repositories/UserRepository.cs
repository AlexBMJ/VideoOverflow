
namespace VideoOverflow.Infrastructure.repositories;

/// <summary>
/// The repository for the user relation
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IVideoOverflowContext _context;

    /// <summary>
    /// Initialize the repository with a given context
    /// </summary>
    /// <param name="context">Context for a DB</param>
    public UserRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all the users from the relation in the DB
    /// </summary>
    /// <returns>A collection of all the users in the DB</returns>
    public async Task<IReadOnlyCollection<UserDTO>> GetAll()
    {
        return await _context.Users.Select(user =>
            new UserDTO(user.Id, user.Name, user.Comments.Select(comment => comment.Content).ToList())).ToListAsync();
    }

    /// <summary>
    /// Gets a specific user based on an id
    /// </summary>
    /// <param name="id">The id of the user to look for</param>
    /// <returns>The user with the specified id or null if such a user doesn't exist</returns>
    public async Task<UserDTO?> Get(int id)
    {
        var entity = await _context.Users.Where(user => user.Id == id).Select(c => c).FirstOrDefaultAsync();

        return entity == null
            ? null
            : new UserDTO(entity.Id, entity.Name, entity.Comments.Select(c => c.Content).ToList());
    }

    /// <summary>
    /// Pushes a user to the relation in the DB
    /// </summary>
    /// <param name="user">The user to push to the database</param>
    /// <returns>The pushed user</returns>
    public async Task<UserDTO> Push(UserCreateDTO user)
    {
        var entity = new User() {Name = user.Name, Comments = new Collection<Comment>() };

        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new UserDTO(entity.Id, entity.Name, new Collection<string>());
    }

    /// <summary>
    /// Updates a user in the relation in the DB
    /// </summary>
    /// <param name="userUpdate">The updated user</param>
    /// <returns>The status of the push</returns>
    public async Task<Status> Update(UserUpdateDTO userUpdate)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(c => c.Id == userUpdate.Id);

        if (entity == null)
        {
            return Status.NotFound;
        }

        if (entity.Name != userUpdate.Name)
        {
            entity.Name = userUpdate.Name;
        }
        
        await _context.SaveChangesAsync();

        return Status.Updated;
    }
}