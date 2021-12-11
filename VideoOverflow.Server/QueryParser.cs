using System.Collections.Generic;
using System.Text;
using Fastenshtein;
using VideoOverflow.Infrastructure.Entities;

namespace Server;
public class QueryParser
{
    private readonly ITagRepository _tagRepo;
    private readonly IResourceRepository _resourceRepo;
    public QueryParser(ITagRepository tagRepo, IResourceRepository resourceRepo) {
        _tagRepo = tagRepo;
        _resourceRepo = resourceRepo;
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

    private string ParseAllWords(string query, IEnumerable<string> tags) {
        var resourceDTO = _resourceRepo.GetAll().Result;
        var minDist = int.MaxValue;
        var min = query;
        foreach (var dto in resourceDTO) {
            var title = dto.SiteTitle;
            var dist = Levenshtein.Distance(title, query);
            if (dist < minDist) {
                minDist = dist;
                min = title;
            }
        }
        return min;
    }
}