namespace VideoOverflow.Core;

public interface IWordRepository
{
    public Task<IReadOnlyCollection<WordDTO>> GetAll();

    public Task<WordDTO> Push(WordCreateDTO word);
}