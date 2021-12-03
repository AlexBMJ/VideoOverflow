using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace VideoOverflow.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IVideoOverflowContext _context;

    public UserRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserDTO>> GetAll()
    {
        return await _context.Users.Select(user =>
            new UserDTO(user.Id, user.Name, user.Comments.Select(comment => comment.Content).ToList())).ToListAsync();
    }

    public async Task<UserDTO?> Get(int id)
    {
        var entity = await _context.Users.Where(user => user.Id == id).Select(c => c).FirstOrDefaultAsync();

        return entity == null
            ? null
            : new UserDTO(entity.Id, entity.Name, entity.Comments.Select(c => c.Content).ToList());
    }

    public async Task<UserDTO> Push(UserCreateDTO user)
    {
        var entity = new User() {Name = user.Name, Comments = new Collection<Comment>() };

        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new UserDTO(entity.Id, entity.Name, new Collection<string>());
    }

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