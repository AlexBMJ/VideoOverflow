using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the comment repository. This ensures all the crud methods are implemented
/// </summary>
public interface ICommentRepository
{
    
    Task<IReadOnlyCollection<CommentDTO>> GetAll();
    Task<CommentDTO?> Get(int id);
    Task<CommentDTO> Push(CommentCreateDTO comment);
    Task<Status> Update(CommentUpdateDTO comment);
}