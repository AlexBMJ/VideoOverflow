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
        return (await (from u in _context.Users
            select new UserDTO(u.Id,
                u.Name,
                u.Comments.Select(c => c.Content).ToList())).ToListAsync()).AsReadOnly();
    }

    public async Task<UserDTO?> Get(int id)
    {
        return await (from u in _context.Users
            where u.Id == id
            select new UserDTO(u.Id, u.Name, u.Comments.Select(c => c.Content).ToList())).FirstOrDefaultAsync();
    }

    public async Task<UserDTO> Push(UserCreateDTO user)
    {
        var comments = new Collection<Comment>();
        
        foreach (var comment in user.Comments)
        {
            comments.Add(new Comment(){Content = comment});
        }
        
        var entity = new User() {Name = user.Name, Comments = comments};

        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new UserDTO(entity.Id, entity.Name, entity.Comments.Select(c => c.Content).ToList());
    }
    public async Task<Status> Update(UserUpdateDTO resource)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(c => c.Id == resource.Id);

        if (entity == null)
        {
            return Status.NotFound;
        }

        ICollection<Comment> comments = new Collection<Comment>();

        foreach (var comment in resource.Comments)
        {
            comments.Add(new Comment(){Content = comment});
        }

        entity.Comments = comments;
        return Status.Updated;
    }
}