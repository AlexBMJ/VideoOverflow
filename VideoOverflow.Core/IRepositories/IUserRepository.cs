using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the user repository. This ensures all the crud methods are implemented
/// </summary>
public interface IUserRepository
{

    Task<IReadOnlyCollection<UserDTO>> GetAll();
    
    Task<UserDTO?> Get(int id);
    
    Task<UserDTO> Push(UserCreateDTO user);

    Task<Status> Update(UserUpdateDTO user);
}