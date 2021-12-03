namespace VideoOverflow.Infrastructure.repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IVideoOverflowContext _context;

    public CommentRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CommentDTO>> GetAll()
    {
        return (await _context.Comments.Select(c => new CommentDTO(c.Id, c.CreatedBy, c.AttachedToResource, c.Content))
                .ToListAsync())
            .AsReadOnly();
    }

    public async Task<CommentDTO?> Get(int commentId)
    {
        return await (from c in _context.Comments
            where c.Id == commentId
            select new CommentDTO(c.Id, c.CreatedBy, c.AttachedToResource, c.Content)).FirstOrDefaultAsync();
    }

    public async Task<CommentDTO> Push(CommentCreateDTO comment)
    {
        var createdComment = new Comment()
            {Content = comment.Content, CreatedBy = comment.CreatedBy, AttachedToResource = comment.AttachedToResource};
        
        foreach (var resource in _context.Resources)
        {
            if (resource.Id == comment.AttachedToResource)
            {
                if (resource.Comments == null)
                {
                    resource.Comments = new Collection<Comment>();
                }
                resource.Comments.Add(createdComment);
            }
        }

        foreach (var user in _context.Users)
        {
            if (user.Id == comment.CreatedBy)
            {
                user.Comments.Add(createdComment);
            }
        }

        await _context.Comments.AddAsync(createdComment);
        await _context.SaveChangesAsync();

        return new CommentDTO(createdComment.Id, createdComment.CreatedBy, createdComment.AttachedToResource,
            createdComment.Content);
    }

    public async Task<Status> Update(CommentUpdateDTO comment)
    {
        var entity = await (from c in _context.Comments
            where c.Id == comment.Id
            select c).FirstOrDefaultAsync();

        if (entity == null)
        {
            return Status.NotFound;
        }

        if (entity.Content != comment.Content)
        {
            entity.Content = comment.Content;
            
            foreach (var resource in _context.Resources)
            {
                foreach (var resourceComment in resource.Comments)
                {
                    if (resourceComment.AttachedToResource == comment.AttachedToResource)
                    {
                        resourceComment.Content = comment.Content;
                    }
                }
            }

            foreach (var user in _context.Users)
            {
                foreach (var userComment in user.Comments)
                {
                    if (userComment.CreatedBy == comment.CreatedBy)
                    {
                        userComment.Content = comment.Content;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        return Status.Updated;
    }
}