using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<UserDTO>> GetAll();
    Task<UserDTO?> Get(int id);
    Task<UserDTO> Push(UserCreateDTO resource);
    Task<Status> Update(UserUpdateDTO resource);
}