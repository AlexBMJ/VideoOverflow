namespace Server;
public class QueryParser
{
    private readonly ITagRepository _tagRepo;
    public QueryParser(ITagRepository tagRepo) {
        _tagRepo = tagRepo;
    }

    public IEnumerable<TagDTO> ParseTags(string query) {
        var ids = new HashSet<int>();
        foreach (var word in query.Split(" ")) {
            var tags = _tagRepo.GetTagByNameAndSynonym(word).Result;
            foreach (var tag in tags) {
                if (ids.Contains(tag.Id)) continue;
                ids.Add(tag.Id);
                yield return tag;
            }
        }
    }
}