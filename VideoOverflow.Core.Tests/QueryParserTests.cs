using System.Collections.Generic;
using System.Linq;

namespace VideoOverflow.Core.Tests;

public class QueryParserTests
{
    private readonly VideoOverflowContext _context;
    private readonly TagRepository _tagRepo;
    private readonly WordRepository _wordRepo;

    public QueryParserTests()
    {
        var repo = new QueryParserTestsSetup();
        _context = repo.Context;

        _tagRepo = new TagRepository(_context);
        _wordRepo = new WordRepository(_context);
    }
    [Fact]
    public void QueryParser_returns_hi_and_john_given_Hi_John_and_tags_Hi_and_John()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        _tagRepo.Push(new TagCreateDTO() {Name = "John", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = new List<string>() {"hi", "john"};
        var actual = parser.Parse("Hi John").ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
    
    [Fact]
    public void QueryParser_returns_hi_given_Hi_John_and_tags_Hi()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = new List<string>() {"hi"};
        var actual = parser.Parse("Hi John").ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected[0], actual[0]);
    }
    [Fact]
    public void QueryParser_suggests_Hi_John_given_He_Jon_and_tags_Hi_and_John_and_no_words()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        _tagRepo.Push(new TagCreateDTO() {Name = "John", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = "Hi John";
        var actual = parser.SuggestQuery("He Jon");
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void QueryParser_suggests_Hi_Johnny_given_Hi_Jonny_and_tag_Johnny_and_word_Hi()
    {
        _wordRepo.Push(new WordCreateDTO(){Hash = "Hi".GetHashCode(), String = "Hi"});
        _tagRepo.Push(new TagCreateDTO() {Name = "Johnny", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = "Hi Johnny";
        var actual = parser.SuggestQuery("Hi Jonny");
        Assert.Equal(expected, actual);
    }
}