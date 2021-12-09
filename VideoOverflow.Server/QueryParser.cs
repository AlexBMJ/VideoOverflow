using System.Collections.Generic;
using System.Text;
using Fastenshtein;
namespace Server;
public class QueryParser
{
    private readonly ITagRepository _tagRepo;
    private readonly IWordRepository _wordRepo;
    public QueryParser(ITagRepository tagRepo, IWordRepository wordRepo) {
        _tagRepo = tagRepo;
        _wordRepo = wordRepo;
    }

    public IEnumerable<string> Parse(string query) {
        var tags = _tagRepo.GetAll().Result;
        var tagNames = new HashSet<string>();
        foreach (var dto in tags) {
            tagNames.Add(dto.Name.ToLower());
        }
        
        foreach (var word in query.ToLower().Split(" ")) {
            if (tagNames.Contains(word)) {
                yield return word;
            }
        }
    }

    public string SuggestQuery(string query) {
        var tagDTOs = _tagRepo.GetAll().Result;
        var tags = new HashSet<string>();
        foreach (var dto in tagDTOs) {
            tags.Add(dto.Name);
        }
        var wordDTOs = _wordRepo.GetAll().Result;
        var words = new HashSet<string>();
        foreach (var dto in wordDTOs) {
            words.Add(dto.String);
        }

        var sb = new StringBuilder();
        foreach (var word in query.Split(" ")) {
            if (!tags.Contains(word) && !words.Contains(word)) {
                sb = sb.Append(ClosestTag(word));
            }
            else {
                sb = sb.Append(word);
            }
            sb = sb.Append(' ');
        }

        sb = sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    private string ClosestTag(string word) {
        var tagDTOs = _tagRepo.GetAll().Result;
        var minDist = int.MaxValue;
        var min = word;
        foreach (var dto in tagDTOs) {
            var name = dto.Name;
            var dist = Levenshtein.Distance(name, word);
            if (dist < minDist)
            {
                minDist = dist;
                min = name;
            }
        }
        return min;
    }
}