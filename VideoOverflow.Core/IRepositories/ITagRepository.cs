using VideoOverflow.Core.DTOs;

namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the tag repository. This ensures all the crud methods are implemented
/// </summary>
public interface ITagRepository
{
    
    /// <summary>
    /// Gets all tags in the database
    /// </summary>
    /// <returns>A collection of all tags in the database</returns>
    public Task<IReadOnlyCollection<TagDTO>> GetAll();
    
    /// <summary>
    /// Gets a tag based on it's name
    /// </summary>
    /// <param name="tagName">The name of the tag to search for</param>
    /// <returns>The tag with the specified name of null if it doesn't exist</returns>
    public Task<TagDTO?> GetTagByName(string tagName);
    
    /// <summary>
    /// Gets a tag by either it's name or one if it's tagSynonyms if it is similar to the input name
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <returns>A collection of all tags which are similar to the input name</returns>
    public Task<IReadOnlyCollection<TagDTO>> GetTagByNameAndSynonym(string name);

    /// <summary>
    /// Gets a tag in the database by it's id
    /// </summary>
    /// <param name="tagId">The id of the tag to search for</param>
    /// <returns>The tag with the specified id or null if it doesn't exist</returns>
    public Task<TagDTO?> Get(int tagId);

    /// <summary>
    /// Pushes a tag to the database
    /// </summary>
    /// <param name="tag">The tag to push</param>
    /// <returns>The pushed tag</returns>
    public Task<TagDTO> Push(TagCreateDTO tag);

    /// <summary>
    /// Updates a tag in the database
    /// </summary>
    /// <param name="update">The updated tag</param>
    /// <returns>The status of the update</returns>
    public Task<Status> Update(TagUpdateDTO update);
}