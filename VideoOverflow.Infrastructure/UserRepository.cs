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
        
        var entity = new User() {Name = user.Name, Comments = new Collection<Comment>()};

        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new UserDTO(entity.Id, entity.Name, entity.Comments.Select(c => c.Content).ToList());
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

        if (userUpdate.Comments != null)
        {
            if (entity.Comments != null)
            {
                entity.Comments = await GetComments(userUpdate.Id, userUpdate.Comments);
            }
        }

        await _context.SaveChangesAsync();
        
        return Status.Updated;
    }

    private async Task<ICollection<Comment>> GetComments(int userId, IEnumerable<string> comments)
    {
        var commentsUpdated = new Collection<Comment>();
        foreach (var comment in comments)
        {
            var exists = await _context.Comments.FirstOrDefaultAsync(c => c.Content == comment && c.CreatedBy == userId);

            if (exists == null)
            {
                exists = new Comment() {CreatedBy = userId, Content = comment};
                await _context.Comments.AddAsync(exists);
                await _context.SaveChangesAsync();
            }
            
            commentsUpdated.Add(exists);
            
        }

        return commentsUpdated;
    }
}