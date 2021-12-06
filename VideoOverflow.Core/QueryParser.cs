namespace VideoOverflow.Core;

public class QueryParser
{
    private ITagRepository _repo;
    public QueryParser(ITagRepository repo)
    {
        _repo = repo;
    }

    public IEnumerable<string> Parse(string query)
    {
        var tags = _repo.GetAll().Result;
        var tagNames = new HashSet<string>();
        foreach (var dto in tags)
        {
            tagNames.Add(dto.Name.ToLower());
        }
        
        foreach (var word in query.ToLower().Split(" "))
        {
            if (tagNames.Contains(word))
            {
                yield return word;
            }
        }
    }

    public string SuggestQuery(string query)
    {
        return "Hi John";
    }
}