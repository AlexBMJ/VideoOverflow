namespace VideoOverflow.Core;

public interface ICommentRepository
{
    
    Task<IReadOnlyCollection<CommentDTO>> GetAll();
    Task<CommentDTO> Get(int id);
    Task<CommentDTO> Push(CommentCreateDTO resource);
    Task<Status> Update(CommentUpdateDTO resource);
}